using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DailyProduction.Model;
using Azure.Data.Tables;
using Azure;


namespace IbasAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DailyProductionController : ControllerBase
    {
        private static TableClient tableClient;
        
        private readonly ILogger<DailyProductionController> _logger;

        public DailyProductionController(ILogger<DailyProductionController> logger)
        {
            _logger = logger;
            var serviceUri = "https://dailyproduction.table.core.windows.net/IBASProduktion2022";
            var tableName = "IBASProduktion2022";
            var accountName = "dailyproduction"; 
            var storageAccountKey = "o6pXUgZKwO33CoBtVLQhi9gmaabEPUuN7Wgo+7p1BhhAsn2Yy+z2RO14/ETBqLXBWmUJ8rGyarv9+ASt0A+6VQ=="; 

            tableClient = new TableClient(
             new Uri(serviceUri),
             tableName,
             new TableSharedKeyCredential(accountName, storageAccountKey));
        }
        [HttpGet]
        public IEnumerable<DailyProductionDTO> Get()
        {
            var _productionrepo = new List<DailyProductionDTO>();
            Pageable<TableEntity> entities = tableClient.Query<TableEntity>();
            foreach (TableEntity entity in entities)
            {
                var dto = new DailyProductionDTO
                {
                    Date = DateTime.Parse(entity.RowKey),
                    Model = (BikeModel)Enum.ToObject(typeof(BikeModel), Int32.Parse(entity.PartitionKey)),
                    ItemsProduced = (int)entity.GetInt32("itemsProduced")
                };
                _productionrepo.Add(dto);
            }
            return _productionrepo;
        }
    }
}