using System;
using System.Collections.Generic;
using System.Security.Policy;
using Skybrud.Umbraco.Feedback.Config;
using Skybrud.Umbraco.Feedback.Constants;
using Skybrud.Umbraco.Feedback.Controllers;
using Skybrud.Umbraco.Feedback.Exceptions;
using Skybrud.Umbraco.Feedback.Interfaces;
using Skybrud.Umbraco.Feedback.Model;
using Skybrud.Umbraco.Feedback.Model.Entries;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;

namespace Skybrud.Umbraco.Feedback {
    
    public class FeedbackModule {

        #region Private fields

        private static FeedbackModule _instance;

        private List<IFeedbackPlugin> _plugins = new List<IFeedbackPlugin>();

        #endregion

        #region Properties

        public static FeedbackModule Instance {
            get { return _instance ?? (_instance = new FeedbackModule()); }
        }
        
        static UmbracoDatabase Database {
            get { return ApplicationContext.Current.DatabaseContext.Database; }
        }

        public IFeedbackPlugin[] Plugins {
            get { return _plugins.ToArray(); }
        }

        /// <summary>
        /// Gets a reference to the entries endpoint.
        /// </summary>
        public FeedbackEntriesRepository Entries { get; private set; }

        #endregion

        #region Constructors

        public FeedbackModule() {
            Entries = new FeedbackEntriesRepository(this);
        }

        #endregion

        public void AddPlugin(IFeedbackPlugin plugin) {
            if (plugin == null) throw new ArgumentNullException("plugin");
            _plugins.Add(plugin);
        }

        /// <summary>
        /// Gets an array of all available ratings for the site with the specified <code>siteId</code>.
        /// </summary>
        /// <param name="siteId">The ID of the site.</param>
        public FeedbackRating[] GetRatingsForSite(int siteId) {
            foreach (IFeedbackPlugin plugin in _plugins) {
                try {
                    FeedbackRating[] ratings = plugin.GetRatingsForSite(this, siteId);
                    if (ratings != null) return ratings;
                } catch (Exception ex) {
                    LogHelper.Error<FeedbackModule>("Plugin of type " + plugin.GetType() + " failed for method GetRatingsForSite(" + siteId + ").", ex);
                }
            }
            return new FeedbackRating[0];
        }

        /// <summary>
        /// Gets an array of all available statuses for the site with the specified <code>siteId</code>.
        /// </summary>
        /// <param name="siteId">The ID of the site.</param>
        public FeedbackStatus[] GetStatusesForSite(int siteId) {
            foreach (IFeedbackPlugin plugin in _plugins) {
                try {
                    FeedbackStatus[] statuses = plugin.GetStatusesForSite(this, siteId);
                    if (statuses != null) return statuses;
                } catch (Exception ex) {
                    LogHelper.Error<FeedbackModule>("Plugin of type " + plugin.GetType() + " failed for method GetStatusesForSite(" + siteId + ").", ex);
                }
            }
            return new FeedbackStatus[0];
        }
        
        public IFeedbackUser GetUser(int userId) {
            foreach (IFeedbackPlugin plugin in _plugins) {
                try {
                    IFeedbackUser user = plugin.GetUser(userId);
                    if (user != null) return user;
                } catch (Exception ex) {
                    LogHelper.Error<FeedbackModule>("Plugin of type " + plugin.GetType() + " failed for method GetUser.", ex);
                }
            }
            return null;
        }

        public Dictionary<int, IFeedbackUser> GetUsers() {
            Dictionary<int, IFeedbackUser> users = new Dictionary<int, IFeedbackUser>();
            foreach (IFeedbackPlugin plugin in _plugins) {
                try {
                    foreach (var user in plugin.GetUsers()) {
                        users.Add(user.Id, user);
                    }
                } catch (Exception ex) {
                    LogHelper.Error<FeedbackModule>("Plugin of type " + plugin.GetType() + " failed for method GetUsers.", ex);
                }
            }
            return users;
        }
        
        public FeedbackEntry AddFeedbackComment(IPublishedContent site, IPublishedContent content, FeedbackProfile profile, FeedbackRating rating, FeedbackStatus status, string name, string email, string comment) {

            FeedbackEntry entry = new FeedbackEntry {
                UniqueId = Guid.NewGuid() + "",
                Site = site,
                Page = content,
                Name = FeedbackUtils.TrimToNull(name),
                Email = FeedbackUtils.TrimToNull(email),
                Rating = rating,
                Comment = FeedbackUtils.TrimToNull(comment),
                Created = DateTime.UtcNow,
                Status = status
            };

            // Attempt to create the database table if it doesn't exist
            try {
                if (!Database.TableExist("SkybrudFeedback")) {
                    Database.CreateTable<FeedbackDatabaseEntry>(false);
                }
            } catch (Exception) {
                throw new FeedbackException("2QW843", "Din feedback kunne ikke tilføjes pga. en fejl på serveren.");
            }

            // Attempt to add the entry to the database
            try {

                // Trigger the "OnEntrySubmitting" before adding the entry
                foreach (IFeedbackPlugin plugin in _plugins) {
                    try {
                        if (!plugin.OnEntrySubmitting(this, entry)) {
                            return null;
                        }
                    } catch (Exception ex) {
                        LogHelper.Error<FeedbackModule>("Plugin of type " + plugin.GetType() + " failed for method OnEntrySubmitting.", ex);
                    }
                }

                // Insert the item into the database
                entry.Insert();

                // Trigger the "OnEntrySubmitted" after the entry has been added
                foreach (IFeedbackPlugin plugin in _plugins) {
                    try {
                        plugin.OnEntrySubmitted(this, entry);
                    } catch (Exception ex) {
                        LogHelper.Error<FeedbackModule>("Plugin of type " + plugin.GetType() + " failed for method OnEntrySubmitted.", ex);
                    }
                }

                return entry;

            } catch (Exception ex) {

                LogHelper.Error<FeedbackModule>("Unable to add feedback entry", ex);

                throw new FeedbackException("NX84U7", "Din feedback kunne ikke tilføjes pga. en fejl på serveren.");
            
            }

        }

        public bool SetAssignedTo(FeedbackEntry entry, IFeedbackUser user) {

            // Some input validation
            if (entry == null) throw new ArgumentNullException("entry");

            // Get the current (old) user
            IFeedbackUser oldUser = entry.AssignedTo;

            // Trigger the "OnUserAssigning" before assigning the user
            foreach (IFeedbackPlugin plugin in _plugins) {
                try {
                    if (!plugin.OnUserAssigning(this, entry, user)) {
                        return false;
                    }
                } catch (Exception ex) {
                    LogHelper.Error<FeedbackModule>("Plugin of type " + plugin.GetType() + " failed for method OnUserAssigning.", ex);
                }
            }

            entry.SetAssignedTo(user);

            // Trigger the "OnUserAssigned" event when the user has been assigned
            foreach (IFeedbackPlugin plugin in _plugins) {
                try {
                    plugin.OnUserAssigned(this, entry, oldUser, user);
                } catch (Exception ex) {
                    LogHelper.Error<FeedbackModule>("Plugin of type " + plugin.GetType() + " failed for method OnUserAssigned.", ex);
                }
            }

            return true;

        }

        public FeedbackEntry AddRating(IPublishedContent site, IPublishedContent page, FeedbackProfile profile, FeedbackRating rating) {

            FeedbackEntry entry = new FeedbackEntry {
                UniqueId = Guid.NewGuid() + "",
                Site = site,
                Page = page,
                Rating = rating,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                Status = FeedbackStatus.New
            };

            // Attempt to create the database table if it doesn't exist
            try {
                if (!Database.TableExist("SkybrudFeedback")) {
                    Database.CreateTable<FeedbackDatabaseEntry>(false);
                }
            } catch (Exception ex) {
                LogHelper.Error<FrontendController>("Error code: " + FeedbackConstants.ErrorCodes.CreateTableFailed, ex);
                throw new FeedbackException(FeedbackConstants.ErrorCodes.CreateTableFailed, FeedbackConstants.ErrorMessages.DefaultSubmitError);
            }


            // Attempt to add the entry to the database
            try {

                // Trigger the "OnRatingSubmitting" before adding the entry
                foreach (IFeedbackPlugin plugin in _plugins) {
                    try {
                        if (!plugin.OnRatingSubmitting(this, entry)) {
                            return null;
                        }
                    } catch (Exception ex) {
                        LogHelper.Error<FeedbackModule>("Plugin of type " + plugin.GetType() + " failed for method OnRatingSubmitting.", ex);
                    }
                }

                // Insert the item into the database
                entry.Insert();

                // Trigger the "OnRatingSubmitted" after the entry has been added
                foreach (IFeedbackPlugin plugin in _plugins) {
                    try {
                        plugin.OnRatingSubmitted(this, entry);
                    } catch (Exception ex) {
                        LogHelper.Error<FeedbackModule>("Plugin of type " + plugin.GetType() + " failed for method OnRatingSubmitted.", ex);
                    }
                }

                return entry;

            } catch (Exception ex) {
                LogHelper.Error<FrontendController>("Error code: " + FeedbackConstants.ErrorCodes.CreateTableFailed, ex);
                throw new FeedbackException(FeedbackConstants.ErrorCodes.InsertEntryFailed, FeedbackConstants.ErrorMessages.DefaultSubmitError);
            }

        }
    
    }

}