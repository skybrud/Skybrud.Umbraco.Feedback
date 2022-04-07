using System;
using System.Linq;
using Skybrud.Umbraco.Feedback.Models.Entries;
using Skybrud.Umbraco.Feedback.Models.Ratings;
using Skybrud.Umbraco.Feedback.Models.Results;
using Skybrud.Umbraco.Feedback.Models.Sites;
using Skybrud.Umbraco.Feedback.Models.Statuses;
using Skybrud.Umbraco.Feedback.Models.Users;
using Skybrud.Umbraco.Feedback.Plugins;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;

namespace Skybrud.Umbraco.Feedback.Services {

    /// <summary>
    /// Service class for working with feedback entries.
    /// </summary>
    public class FeedbackService {
        
        private readonly ILogger _logger;
        
        private readonly FeedbackDatabaseService _databaseService;
        
        #region Properties

        protected FeedbackPluginCollection Plugins => FeedbackPluginCollection.Current;

        #endregion

        #region Constructors

        public FeedbackService(ILogger logger, FeedbackDatabaseService databaseService) {
            _logger = logger;
            _databaseService = databaseService;
        }

        #endregion

        #region Public methods

        public bool TryGetSite(Guid key, out FeedbackSiteSettings site) {
            return Plugins.TryGetSite(key, out site);
        }

        public bool TryGetUser(Guid key, out IFeedbackUser user) {
            foreach (var plugin in Plugins) {
                if (plugin.TryGetUser(key, out user)) {
                    return true;
                }
            }
            user = null;
            return false;
        }
        
        public IFeedbackUser[] GetUsers() {
            return Plugins.SelectMany(x => x.GetUsers()).Distinct().OrderBy(x => x.Name).ToArray();
        }

        ///// <summary>
        ///// Gets an unpaginated array of all feedback entries.
        ///// </summary>
        ///// <returns>An array of <see cref="FeedbackEntry"/>.</returns>
        //public FeedbackEntry[] GetAll() {

        //    // Call this to make sure the users have been loaded before quering the database
        //    Dictionary<int, IFeedbackUser> users = _feedbackModule.GetUsers();

        //    using (var scope = _scopeProvider.CreateScope()) {

        //        // Declare the SQL for the query
        //        var sql = new Sql("SELECT * FROM SkybrudFeedback WHERE Archived = 0 ORDER BY created DESC");

        //        // Make the call to the database
        //        return scope.Database
        //            .Fetch<FeedbackEntryDto>(sql)
        //            .Select(x => new FeedbackEntry(x, users))
        //            .ToArray();

        //    }

        //}




        public FeedbackEntryList GetEntries(FeedbackGetEntriesOptions options) {

            // Look up the entries in the database
            var entries = _databaseService
                .GetEntries(options, out int total)
                .Select(x => new FeedbackEntry(x))
                .ToArray();

            return new FeedbackEntryList(options.Page, options.PerPage, total, entries);

        }
        public FeedbackEntryList GetEntriesForSite(Guid guid, int limit = 0, int page = 0) {

            // Look up the entries in the database
            var entries = _databaseService
                .GetEntriesForSite(guid, limit, Math.Max(1, page), out int total)
                .Select(x => new FeedbackEntry(x))
                .ToArray();

            return new FeedbackEntryList(page, limit, total, entries);

        }

        public FeedbackEntry GetEntryByKey(Guid key) {
            
            FeedbackEntryDto dto = _databaseService.GetEntryByKey(key);
            if (dto == null) return null;

            TryGetSite(dto.SiteKey, out var site);

            FeedbackRating rating = null;
            site?.TryGetRating(dto.Rating, out rating);

            FeedbackStatus status = null;
            site?.TryGetStatus(dto.Status, out status);

            IFeedbackUser user = null;
            TryGetUser(dto.AssignedTo, out user);

            return new FeedbackEntry(dto, rating, status, user);

        }

        /// <summary>
        /// Archives the specified <paramref name="entry"/>.
        /// </summary>
        /// <param name="entry">The entry to be archived.</param>
        public void Archive(FeedbackEntry entry) {
            
            if (entry == null) throw new ArgumentNullException(nameof(entry));
            
            entry.IsArchived = true;

            _databaseService.Update(entry._entry);

        }


        ///// <summary>
        ///// Updates the status of the specified <paramref name="entry"/>.
        ///// </summary>
        ///// <param name="entry">The entry to be updated.</param>
        ///// <param name="status">The new status.</param>
        //public void SetStatus(FeedbackEntry entry, FeedbackStatus status) {
        //    if (entry == null) throw new ArgumentNullException(nameof(entry));
        //    if (status == null) throw new ArgumentNullException(nameof(status));
        //    entry.Status = status;
        //    Save(entry._entry);
        //}

        ///// <summary>
        ///// Updates the assigned user of the specified <paramref name="entry"/>.
        ///// </summary>
        ///// <param name="entry">The entry to be updated.</param>
        ///// <param name="user">The user who should be assigned. Use <c>null</c> if the item should be unassigned.</param>
        //public void SetAssignedTo(FeedbackEntry entry, IFeedbackUser user) {
        //    if (entry == null) throw new ArgumentNullException(nameof(entry));
        //    entry.AssignedTo = user;
        //    Save(entry._entry);
        //}

        /// <summary>
        /// Deletes the specified <paramref name="entry"/>.
        /// </summary>
        /// <param name="entry">The entry to be delete.</param>
        public void Delete(FeedbackEntry entry) {
            if (entry == null) throw new ArgumentNullException(nameof(entry));
            _databaseService.Delete(entry._entry);
        }

        ///// <summary>
        ///// Deletes all entries before the specified <paramref name="date"/>.
        ///// </summary>
        ///// <param name="date">The date.</param>
        ///// <returns>The amount of affected/deleted rows.</returns>
        //public int DeleteAll(DateTime date) {

        //    using (var scope = _scopeProvider.CreateScope()) {

        //        // Delete everything before the start of the day after "date"
        //        Sql sql = new Sql($"DELETE FROM {FeedbackConstants.TableName} WHERE Created < '{date.Date.AddDays(1):yyyy-MM-dd}';");

        //        // Make the call to the database
        //        return scope.Database.Execute(sql);

        //    }

        //}

        ///// <summary>
        ///// Returns the user with the specified <paramref name="userId"/>, or <c>null</c> if not found.
        ///// </summary>
        ///// <param name="userId">The numeric ID of the user.</param>
        ///// <returns>An instance of <see cref="IFeedbackUser"/>, or <c>null</c> if not found.</returns>
        //public IFeedbackUser GetUser(int userId) {
        //    foreach (IFeedbackPlugin plugin in Plugins) {
        //        try {
        //            IFeedbackUser user = plugin.GetUser(userId);
        //            if (user != null) return user;
        //        } catch (Exception ex) {
        //            _logger.Error<FeedbackService>(ex, "Plugin of type {PluginType} failed for method GetUser.", plugin.GetType().FullName);
        //        }
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Returns a dictionary will the users of all available plugins.
        ///// </summary>
        ///// <returns>An dictionary containing the users.</returns>
        //public Dictionary<int, IFeedbackUser> GetUsers() {
        //    Dictionary<int, IFeedbackUser> users = new Dictionary<int, IFeedbackUser>();
        //    foreach (IFeedbackPlugin plugin in Plugins) {
        //        try {
        //            foreach (IFeedbackUser user in plugin.GetUsers()) {
        //                users.Add(user.Id, user);
        //            }
        //        } catch (Exception ex) {
        //            _logger.Error<FeedbackService>(ex, "Plugin of type {PluginType} failed for method GetUsers.", plugin.GetType().FullName);
        //        }
        //    }
        //    return users;
        //}

        ///// <summary>
        ///// Gets an array of all available ratings for the site with the specified <paramref name="siteId"/>.
        ///// </summary>
        ///// <param name="siteId">The ID of the site.</param>
        //public FeedbackRating[] GetRatingsForSite(int siteId) {
        //    foreach (IFeedbackPlugin plugin in Plugins) {
        //        try {
        //            FeedbackRating[] ratings = plugin.GetRatingsForSite(this, siteId);
        //            if (ratings != null) return ratings;
        //        } catch (Exception ex) {
        //            _logger.Error<FeedbackService>(ex, "Plugin of type {PluginType} failed for method GetRatingsForSite({SiteId}).", plugin.GetType().FullName, siteId);
        //        }
        //    }
        //    return new FeedbackRating[0];
        //}

        ///// <summary>
        ///// Gets an array of all available statuses for the site with the specified <paramref name="siteId"/>.
        ///// </summary>
        ///// <param name="siteId">The ID of the site.</param>
        //public FeedbackStatus[] GetStatusesForSite(int siteId) {
        //    foreach (IFeedbackPlugin plugin in Plugins) {
        //        try {
        //            FeedbackStatus[] statuses = plugin.GetStatusesForSite(this, siteId);
        //            if (statuses != null) return statuses;
        //        } catch (Exception ex) {
        //            _logger.Error<FeedbackService>(ex, "Plugin of type {PluginType} failed for method GetStatusesForSite({SiteId}).", plugin.GetType().FullName, siteId);
        //        }
        //    }
        //    return new FeedbackStatus[0];
        //}



        public AddRatingResult AddRating(FeedbackSiteSettings site, IPublishedContent page, FeedbackRating rating) {
            
            // Initialize a new entry
            FeedbackEntry entry = new FeedbackEntry {
                Key = Guid.NewGuid(),
                SiteKey = site.Key,
                PageKey = page.Key,
                Rating = rating,
                Status = site.Statuses.FirstOrDefault(),
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            // Attempt to add the entry to the database
            try {

                // Trigger the "OnRatingSubmitting" before adding the entry
                foreach (IFeedbackPlugin plugin in Plugins) {
                    try {
                        if (!plugin.OnRatingSubmitting(this, entry)) {
                            return AddRatingResult.Cancelled("The feedback submission was cancelled by the server.");
                        }
                    } catch (Exception ex) {
                        _logger.Error<FeedbackService>(ex, "Plugin of type {PluginType} failed for method OnRatingSubmitting.", plugin.GetType().FullName);
                    }
                }

                // Insert the item into the database
                _databaseService.Insert(entry.Dto);

                // Trigger the "OnRatingSubmitted" after the entry has been added
                foreach (IFeedbackPlugin plugin in Plugins) {
                    try {
                        plugin.OnRatingSubmitted(this, entry);
                    } catch (Exception ex) {
                        _logger.Error<FeedbackService>(ex, "Plugin of type {PluginType} failed for method OnRatingSubmitted.", plugin.GetType().FullName);
                    }
                }

                return AddRatingResult.Success(entry);

            } catch (Exception ex) {

                _logger.Error<FeedbackService>(ex, "Unable to add feedback entry.");
                
                return AddRatingResult.Cancelled("The feedback submission could not be saved due to an error on the server.");
            
            }

        }

        public UpdateEntryResult UpdateEntry(FeedbackEntry entry) {
            
            // Ensure the string values are NULL (opposed to empty or white space)
            entry.Name = FeedbackUtils.TrimToNull(entry.Name);
            entry.Email = FeedbackUtils.TrimToNull(entry.Email);
            entry.Comment = FeedbackUtils.TrimToNull(entry.Comment);

            // Attempt to add the entry to the database
            try {

                // Trigger the "OnEntryUpdating" before adding the entry
                foreach (IFeedbackPlugin plugin in Plugins) {
                    try {
                        if (!plugin.OnEntryUpdating(this, entry))  {
                            return UpdateEntryResult.Cancelled("The feedback submission was cancelled by the server.");
                        }
                    } catch (Exception ex) {
                        _logger.Error<FeedbackService>(ex, "Plugin of type {PluginType} failed for method OnEntryUpdating.", plugin.GetType().FullName);
                    }
                }

                // Insert the item into the database
                _databaseService.Update(entry.Dto);

                // Trigger the "OnEntryUpdated" after the entry has been added
                foreach (IFeedbackPlugin plugin in Plugins) {
                    try {
                        plugin.OnEntryUpdated(this, entry);
                    } catch (Exception ex) {
                        _logger.Error<FeedbackService>(ex, "Plugin of type {PluginType} failed for method OnEntryUpdated.", plugin.GetType().FullName);
                    }
                }

                return UpdateEntryResult.Success(entry);

            } catch (Exception ex) {

                _logger.Error<FeedbackService>(ex, "Unable to add feedback entry.");

                return UpdateEntryResult.Cancelled("The feedback submission could not be updated due to an error on the server.");

            }

        }

        public AddCommentResult AddComment(FeedbackSiteSettings site, IPublishedContent page, FeedbackRating rating, string name, string email, string comment) {
            
            FeedbackEntry entry = new FeedbackEntry {
                Key = Guid.NewGuid(),
                SiteKey = site.Key,
                PageKey = page.Key,
                Rating = rating,
                Status = site.Statuses.FirstOrDefault(),
                Name = FeedbackUtils.TrimToNull(name),
                Email = FeedbackUtils.TrimToNull(email),
                Comment = FeedbackUtils.TrimToNull(comment),
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            // Attempt to add the entry to the database
            try {

                // Trigger the "OnEntrySubmitting" before adding the entry
                foreach (IFeedbackPlugin plugin in Plugins) {
                    try {
                        if (!plugin.OnEntrySubmitting(this, entry))  {
                            return AddCommentResult.Cancelled("The feedback submission was cancelled by the server.");
                        }
                    } catch (Exception ex) {
                        _logger.Error<FeedbackService>(ex, "Plugin of type {PluginType} failed for method OnEntrySubmitting.", plugin.GetType().FullName);
                    }
                }

                // Insert the item into the database
                _databaseService.Insert(entry.Dto);

                // Trigger the "OnEntrySubmitted" after the entry has been added
                foreach (IFeedbackPlugin plugin in Plugins) {
                    try {
                        plugin.OnEntrySubmitted(this, entry);
                    } catch (Exception ex) {
                        _logger.Error<FeedbackService>(ex, "Plugin of type {PluginType} failed for method OnEntrySubmitted.", plugin.GetType().FullName);
                    }
                }

                return AddCommentResult.Success(entry);

            } catch (Exception ex) {

                _logger.Error<FeedbackService>(ex, "Unable to add feedback entry.");

                return AddCommentResult.Cancelled("The feedback submission could not be saved due to an error on the server.");

            }

        }

        public bool SetAssignedTo(FeedbackEntry entry, IFeedbackUser user) {

            // Some input validation
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            // Get the current (old) user
            IFeedbackUser oldUser = entry.AssignedTo;
            IFeedbackUser newUser = user;

            // Trigger the "OnUserAssigning" before assigning the user
            foreach (IFeedbackPlugin plugin in Plugins) {
                try {
                    if (!plugin.OnUserAssigning(this, entry, newUser)) {
                        return false;
                    }
                } catch (Exception ex) {
                    _logger.Error<FeedbackService>(ex, "Plugin of type {PluginType} failed for method OnUserAssigning.", plugin.GetType().FullName);
                }
            }

            entry.AssignedTo = newUser;
            entry.UpdateDate = DateTime.UtcNow;

            _databaseService.Update(entry.Dto);

            // Trigger the "OnUserAssigned" event when the user has been assigned
            foreach (IFeedbackPlugin plugin in Plugins) {
                try {
                    plugin.OnUserAssigned(this, entry, oldUser, newUser);
                } catch (Exception ex) {
                    _logger.Error<FeedbackService>(ex, "Plugin of type {PluginType} failed for method OnUserAssigned.", plugin.GetType().FullName);
                }
            }

            return true;

        }

        
        public bool SetStatus(FeedbackEntry entry, FeedbackStatus status) {

            // Some input validation
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            // Get the current (old) status
            FeedbackStatus oldStatus = entry.Status;

            // Trigger the "OnStatusChanging" before chaging the status
            foreach (IFeedbackPlugin plugin in Plugins) {
                try {
                    if (!plugin.OnStatusChanging(this, entry, status)) {
                        return false;
                    }
                } catch (Exception ex) {
                    _logger.Error<FeedbackService>(ex, "Plugin of type {PluginType} failed for method OnStatusChanging.", plugin.GetType().FullName);
                }
            }

            entry.Status = status;
            entry.UpdateDate = DateTime.UtcNow;

            _databaseService.Update(entry.Dto);

            // Trigger the "OnStatusChanged" event when the user has been assigned
            foreach (IFeedbackPlugin plugin in Plugins) {
                try {
                    plugin.OnStatusChanged(this, entry, oldStatus, status);
                } catch (Exception ex) {
                    _logger.Error<FeedbackService>(ex, "Plugin of type {PluginType} failed for method OnStatusChanged.", plugin.GetType().FullName);
                }
            }

            return true;

        }


        /// <summary>
        /// Deletes all entries older than the specified amount of <paramref name="days"/>.
        /// </summary>
        /// <param name="days">The amount of days.</param>
        /// <returns>The amount of affected/deleted rows.</returns>
        public int DeleteAll(int days) {
            return DeleteAll(DateTime.Now.AddDays(-days));
        }
        
        /// <summary>
        /// Deletes all entries before the specified <paramref name="date"/>.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The amount of affected/deleted rows.</returns>
        public int DeleteAll(DateTime date) {
            return _databaseService.DeleteAll(date);
        }

        #endregion

    }

}