using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecieptProcessor.Models;
using RecieptProcessor.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptsController : ControllerBase
    {
       
            private readonly ReceiptProcessingService _receiptProcessingService;

            public ReceiptsController(ReceiptProcessingService receiptProcessingService)
            {
                _receiptProcessingService = receiptProcessingService;
            }

            [HttpPost("process")]
            public IActionResult ProcessReceipt([FromBody] Receipt receipt)
            {
                var id = _receiptProcessingService.ProcessReceipt(receipt);
                return Ok(new { id });
            }

            [HttpGet("{id}/points")]
            public IActionResult GetReceiptPoints(string id)
            {
                var processedReceipt = _receiptProcessingService.GetReceiptPoints(id);
                if (processedReceipt == null)
                {
                    return NotFound();
                }

                return Ok(new { points = processedReceipt.Points });
            }
        }
    }

