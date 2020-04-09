using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddOData();
            services.AddODataQueryFilter();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory,
            IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            // initialize the ClusterHelper
            ClusterHelper.Initialize(
                new ClientConfiguration
                {
                    Servers = new List<Uri>
                    {
                        new Uri("http://localhost:8091/")
                    }
                },
                new PasswordAuthenticator("Administrator", "password"));

            // When application is stopped gracefully shutdown Couchbase connections
            applicationLifetime.ApplicationStopped.Register(() =>
            {
                ClusterHelper.Close();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.Select().Expand().Filter().OrderBy().Count().MaxTop(50); 
                // Select() not supported:
                // Couchbase.Linq.CouchbaseQueryException: An error occurred executing the N1QL query.  See the inner exception for details.
                //  ---> System.NotImplementedException: The method or operation is not implemented.
                //    at Microsoft.AspNet.OData.Query.Expressions.SelectExpandWrapperConverter.ReadJson(
                //    Couchbase.N1QL.QueryClient.ExecuteQueryAsync[T](IQueryRequest queryRequest, CancellationToken cancellationToken)
                endpoints.MapODataRoute("odata", "odata", this.GetEdmModel());
            });
        }
        
        /// <summary>
        /// The Entity Data Model (EDM) is a set of concepts that describe the structure of data,
        /// regardless of its stored form.
        /// https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/entity-data-model
        /// </summary>
        /// <returns>OData model.</returns>
        private IEdmModel GetEdmModel()
        {
            // https://docs.microsoft.com/en-us/odata/webapi/convention-model-builder
            var odataBuilder = new ODataConventionModelBuilder();

            odataBuilder.EntitySet<AirLine>(nameof(AirLine));

            var model = odataBuilder.GetEdmModel();

            return model;
        }
    }
}
