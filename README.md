# Feedback-modul til Umbraco 7 #

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