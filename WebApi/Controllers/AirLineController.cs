using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApi.Controllers
{
    public class AirLineController : ODataController
    {
        private readonly ILogger<AirLineController> logger;

        private readonly IBucketContext bucketContext;

        public AirLineController(ILogger<AirLineController> logger)
        {
            this.logger = logger; 
            this.bucketContext = new BucketContext(ClusterHelper.GetBucket("travel-sample"));
        }
        
        // Works: http://localhost:5000/odata/airline?$filter=Country eq 'United Kingdom'
        // Fails: http://localhost:5000/odata/airline?$filter=Country eq 'United Kingdom'&$select=Id,Name
        [EnableQuery]
        public IQueryable<AirLine> Get()
        {
            return this.bucketContext.Query<AirLine>();
        }

        // GET localhost:5000/odata/airline('10')
        [ODataRoute("AirLine({key})")]
        public ActionResult<AirLine> Get([FromODataUri] string key)
        {
            // does not work on example
            var airline = this.bucketContext.Query<AirLine>().Where(c => c.Id == key);

            this.logger.LogInformation(airline.Expression.ToString());

            if (airline == null)
            {
                return NotFound();
            }

            return Ok(airline);
        }
    }
}
