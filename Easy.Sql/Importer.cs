using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Easy.Sql {
    public class Importer {
        private CompositionContainer mContainer;

        public int AvailableNumberOfOperation => mContainer?.Catalog.Count() ?? 0;

        public void Initialize() {
            var catalog = new AggregateCatalog(
                new DirectoryCatalog(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));
            mContainer = new CompositionContainer(catalog);

            var batch = new CompositionBatch();
            batch.AddExportedValue(mContainer);

            mContainer.Compose(batch);

            IoC.GetInstance = GetInstance;
            IoC.GetAllInstances = GetAllInstances;
            IoC.BuildUp = BuildUp;
        }

        public object GetInstance(Type serviceType, String key) {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = mContainer.GetExportedValues<object>(contract);

            if (exports.Any())
                return exports.First();

            throw new Exception($"Could not locate any instances of contract {contract}.");
        }

        protected IEnumerable<object> GetAllInstances(Type serviceType) {
            return mContainer.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected void BuildUp(object instance) {
            mContainer.SatisfyImportsOnce(instance);
        }
    }
}