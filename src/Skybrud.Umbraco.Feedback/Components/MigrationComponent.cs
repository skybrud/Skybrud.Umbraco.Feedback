using Skybrud.Umbraco.Feedback.Migrations;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Umbraco.Core.Migrations.Upgrade;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;

namespace Skybrud.Umbraco.Feedback.Components {
    
    public class MigrationComponent : IComponent {
        
        private readonly IScopeProvider _scopeProvider;
        private readonly IMigrationBuilder _migrationBuilder;
        private readonly IKeyValueService _keyValueService;
        private readonly ILogger _logger;

        public MigrationComponent(IScopeProvider scopeProvider, IMigrationBuilder migrationBuilder, IKeyValueService keyValueService, ILogger logger) {
            _scopeProvider = scopeProvider;
            _migrationBuilder = migrationBuilder;
            _keyValueService = keyValueService;
            _logger = logger;
        }
        
        public void Initialize() {
            
            var plan = new MigrationPlan("Skybrud.Umbraco.Feedback");
            
            plan.From(string.Empty)
                .To<CreateTableMigration>("3.0.0-alpha001");

            var upgrader = new Upgrader(plan);
            
            upgrader.Execute(_scopeProvider, _migrationBuilder, _keyValueService, _logger);

        }

        public void Terminate() { }

    }

}