# Feedback-modul til Umbraco 7 #

### Brug i frontend ###

Her et eksempel, der tager udgangspunkt i Aabenraa Kommune's løsning.

Felterne *navn*, *email* og *comment* er alle valgfri. Feltet *rating* kan konfigureres med ønskede muligheder for bedømmelse. Standard i modulet er *tilfreds*, *neutral* og *utilfreds*. 

```javascript
$.ajax({
    url: '/umbraco/Feedback/Frontend/PostFeedback',
    method: 'POST',
    data: {
        siteId: $('body').data('siteid'),
        pageId: $('body').data('pageid'),
        name: 'Filip Bech Bruun-Larsen',
        email: 'fbruun@skybrud.dk',
        rating: 'tilfreds',
        comment: 'Ham Bjerner er faktisk ret flink'
    }
}).done(function(r){

    console.log( JSON.stringify(r, null, '  ') );

});
```

Såfremt ovenstående AJAX-kald bliver gennemført, vil serveren svare tilbage noget i stil med følgende:

`{ "meta": { "code": 200 }, "data": 26 }`

Det vigtigste her er koden, hvor 200 betyder gennemført. Værdien under data er kommentarens ID, men værdien bruges pt. ikke til noget, og data vil måske blive ændret i fremtiden til at returnere noget andet.

Tilsvarende hvis noget går galt, vil serveren svare tilbage noget i stil med:

`{ "meta": { "code": 500, "error": "Øv!" }, "data": null }`

Værdien i error vil være en fejlbeskrivelse på dansk, som gerne må vises til brugeren.