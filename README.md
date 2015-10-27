# Feedback-modul til Umbraco 7 #

**Indholdsfortegnelse**

* [Brug i frontend](#markdown-header-brug-i-frontend)
* [Plugins](#markdown-header-plugins)

## Brug i frontend ##

Her et eksempel, der tager udgangspunkt i DC intranet's løsning.

Felterne *name*, *email* og *comment* er som udgangspunkt alle valgfri, men kan konfigureres til, at skulle udfyldes. Feltet *rating* kan konfigureres med ønskede muligheder for bedømmelse. Standard i modulet er *positive* og *negative*. 

```javascript
$.ajax({
    url: '/umbraco/Feedback/Frontend/PostFeedback',
    method: 'POST',
    data: {
        siteId: $('body').data('siteid'),
        pageId: $('body').data('pageid'),
        name: 'Filip Bech Bruun-Larsen',
        email: 'fbruun@skybrud.dk',
        rating: 'positive',
        comment: 'Ham Bjerner er faktisk ret flink'
    }
}).done(function(r){

    console.log( JSON.stringify(r, null, '  ') );

}).error(function(r){

    console.log( JSON.stringify(r, null, '  ') );

});
```

Såfremt ovenstående AJAX-kald bliver gennemført, vil serveren svare tilbage noget i stil med følgende:

`{ "meta": { "code": 200 }, "data": 26 }`

Det vigtigste her er koden, hvor 200 betyder gennemført. Værdien under data er kommentarens ID, men værdien bruges pt. ikke til noget, og data vil måske blive ændret i fremtiden til at returnere noget andet.

Tilsvarende hvis noget går galt, vil serveren svare tilbage noget i stil med:

`{ "meta": { "code": 500, "error": "Øv!" }, "data": null }`

Værdien i error vil være en fejlbeskrivelse, som gerne må vises til brugeren.

## Plugins

Konfigurationen af Feedback-modulet sker gennem kode, hvor man i så fald kan lave en klasse, der implementerer interfacet `IFeedbackPlugin` (eller blot nedarver fra den abstrakte klasse `IFeedbackPlugin`).

Et custom plugin kan registreres som følgende:

```
#!C
namespace MySite {
    
    public class Startup : ApplicationEventHandler {

        protected override void ApplicationStarted(UmbracoApplicationBase umbraco, ApplicationContext application) {

            // Register an instance of the feedback plugin
            FeedbackModule.Instance.AddPlugin(new MySiteFeedbackPlugin());
        
        }

    }

}
```

Teknisk set behøver man ikke tilføje sit plugin over startup, men det giver nok mest mening.

### GetUsers

Feedback-modulet holdet styr på en liste over de brugere, der må kunne vælges som ansvarlig på den enkelte feedback-kommentar. Listen over brugere er dog som udgangspunkt tom, og det er derfor nødvendigt, at definere dette via et custom plugin og metoden `GetUsers`.

Tanken er, at en bruger kan komme fra en eller flere datakilder, som så illustreres via interfacet `IFeedbackUser` (eller den abstrakte klasse `FeedbackUser`). I langt de fleste tilfælde er vi blot interesseret i, at hente en liste over de brugere, der er oprettet i Umbraco. Så i et konkret eksempel kunne `GetUsers` metoden implementeres som nedenstående:

```
#!C
public override IFeedbackUser[] GetUsers() {
            
    // Attempt to get the dictionary from the items object (the dictionary is kept in the items during the current page cycle)
    var users = HttpContext.Current.Items["IntranetFeedbackUsers"] as Dictionary<int, IFeedbackUser>;

    // Check whether we already have a dictionary in memory
    if (users == null) {
                
        // Get a reference to the user service
        IUserService service = UmbracoContext.Current.Application.Services.UserService;

        int total;

        // Populate the dictionary with users from the database
        users = (
            from IUser user in service.GetAll(0, Int32.MaxValue, out total)
            where !user.Email.EndsWith("@skybrud.dk") && !user.Email.EndsWith("@umbraco.com") && !user.Email.EndsWith("@codemonkeys.dk")
            select (IFeedbackUser) new FeedbackUser {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Language = user.Language
            }
        ).ToDictionary(x => x.Id, x => x);

        // Add the dictionary to the items object for later retrieval
        HttpContext.Current.Items["IntranetFeedbackUsers"] = users;
            
    }

    // Return the users as an array
    return users.Values.ToArray();
        
}
```

Bemærk at brugere med en emailadresse, der ender på *@skybrud.dk*, *@umbraco.com* eller *@codemonkeys.dk* i dette eksempel vil blive ekskluderet fra listen.

### OnEntrySubmitting

Via metoden `OnEntrySubmitting` kan vi ændre kommentarer idet de oprettes - altså svarende til inden de bliver skrevet ned i databasen. Dette kunne eksempelvis være at automatisk tilknytte en bruger/redaktør til en kommentar alt efter hvem der er ansvarlig for det site eller den side, hvortil kommentaren gives.

Som eksempel har vi på Danish Crown's internet følgende implementation af `OnEntrySubmitting`-metoden:

```
#!C#
public override bool OnEntrySubmitting(FeedbackModule module, FeedbackEntry entry) {

    // Specifikt til DC's intranet (her tjekkes om kommentarer oprettes under et intranet site)
    if (entry.Site == null || entry.Site.DocumentTypeAlias != "SettingsContainer") return false;

    // Henter en liste over alle brugere (også fra andre plugins såfremt der er flere)
    Dictionary<int, IFeedbackUser> users = GetUsers().ToDictionary(x => x.Id, x => x);

    // Igen lidt specifikt til DC (her hentes et entry for hvem, der sidst har redigeret den pågældende side)
    ContentReminderLogEntry logEntry = ContentReminderLogEntry.GetFromNodeId(entry.PageId).FirstOrDefault(x => users.ContainsKey(x.UserId));

    if (logEntry != null) {
    
        // Såfremt vi finder en entry, assigner vi brugeren til kommentaren
        entry.AssignedTo = users[logEntry.UserId];
    
    } else {

        // På hvert intranet site har DC mulighed for, at angive en ansvarlig
        string value = entry.Site.GetPropertyValue<string>("feedbackResponsible");

        if (!String.IsNullOrWhiteSpace(value) && value.StartsWith("{") && value.EndsWith("}")) {
            try {

                // Hent den valgte brugers ID ud fra JSON-objektet
                JObject obj = JObject.Parse(value);
                int responsibleId = (int) obj.GetValue("id");

                // Assign kommentarer til brugeren såfremt brugeren findes
                if (users.ContainsKey(responsibleId)) {
                    entry.AssignedTo = users[responsibleId];
                }
            
            } catch (Exception) {

                // Ignorér exception eller gem i en log et sted

            }

        }

    }

    return true;

}
```

Denne metode kan også bruges til at afvise en kommentar, hvis eksempelvis emailadressen ikke er udfyldt.

### OnEntrySubmitted

Tilsvarende kan man med `OnEntrySubmitted`-metoden fange når en kommentar er blevet gemt i databasen. Denne metode kunne eksempelvis bruges til, at udsende en mail til den rekdaktør, der er blevet assigned jvf. ovenstående metode.

```
#!C
public override void OnEntrySubmitted(FeedbackModule module, FeedbackEntry entry) {

     // Skip now if not assigned
    if (entry.AssignedTo == null) return;

    // Get the first name of the user
    string firstName = entry.AssignedTo.Name.Split(' ')[0];

    // Generate the body of the mail to be sent (DescribeEntry is a custom method)
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("Hi " + firstName + " (" + entry.AssignedTo.Email + "),<br />");
    sb.AppendLine("<br />");
    sb.AppendLine("A new feedback entry has been submitted to which you have been assigned:<br />");
    sb.AppendLine("<br />");
    DescribeEntry(sb, entry);

    // Send the mail to the user (again a custom user)
    SendMail("DC Intra - New feedback entry", entry.AssignedTo, sb.ToString());

}
```

### OnUserAssigned

Når en bruger efter oprettelsen assignes til en kommantar, bliver `OnUserAssigned` kaldt for den pågældende kommentar. Metoden har en henvisning til både den bruger, der tidligere var assigned kommentaren, og den bruger der netop er blevet assigned.

Typisk kunne man have brug for, at sende en mail til den bruger, der bliver assigned - dette kan i så fald gøres med nedenstående kode:

```
#!C
public override void OnUserAssigned(FeedbackModule module, FeedbackEntry entry, IFeedbackUser oldUser, IFeedbackUser newUser) {

    // Just skip now if no user was assigned
    if (newUser == null) return;

    // Skip if the new user is the same as the old user
    if (oldUser != null && oldUser.Id == newUser.Id) return;

    // Get the first name of the user
    string firstName = newUser.Name.Split(' ')[0];

    // Generate the body of the mail to be sent (DescribeEntry is a custom method)
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("Hi " + firstName + " (" + newUser.Email + "),<br />");
    sb.AppendLine("<br />");
    sb.AppendLine("You have been assigned the following feedback entry:<br />");
    sb.AppendLine("<br />");
    DescribeEntry(sb, entry);

    // Send the mail to the user (again a custom user)
    SendMail("DC Intra - You were assigned to a feedback entry", entry.AssignedTo, sb.ToString());

}
```

### OnEntryResultRender

Har vi brug for at ændre værdierne for de kommentarer, der vises i listen i backoffice, kan dette gøres via `OnEntryResultRender`-metoden. Et eksempel kunne her være virtuelle URL'er på DC's intranet:


```
#!C
public override void OnEntryResultRender(FeedbackModule module, FeedbackEntryResult result) {

    // If the page wasn't found in the Umbraco cache, we don't bother looking up the virtual URL
    if (result.Page == null) return;

    // Check for document types using URL rewriting
    switch (result.Page.Content.DocumentTypeAlias) {
        case IntranetConstants.DocumentTypes.IntranetNewsPage:
        case IntranetConstants.DocumentTypes.IntranetBlogger:
        case IntranetConstants.DocumentTypes.IntranetBlogPage:
        case IntranetConstants.DocumentTypes.IntranetActivityPage: {

            // Lookup the parent site in the Umbraco cache
            IPublishedContent site = UmbracoContext.Current.ContentCache.GetById(result.SiteId);

            if (site == null) {
                result.Page.Url = null;
            } else {

                string url = result.Page.Content.UrlWithDomain();

                int pos1 = url.IndexOf("//");
                int pos2 = pos1 >= 0 ? url.IndexOf('/', pos1 + 2) : url.IndexOf('/');

                // Calculate the virtual URL
                string domain = (pos2 > 0 ? url.Substring(0, pos2) : "");
                result.Page.Url = domain + Utils.VirtualPages.GetVirtualUrl(result.Page.Content, IntranetOverview.GetFromContent(site));

            }

            break;

        }

    }

}
```