using ExcelDataReader;
using ManageCourse.Core.Data;
using ManageCourse.Core.Repositories;
using ManageCourseAPI.Controllers.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Controllers
{
    [ApiController]
    [Route("Grade")]
    public class GradeController : ApiControllerBase
    {
        public GradeController(
            IGeneralModelRepository generalModelRepository, 
            DbContextContainer dbContextContainer) : base(generalModelRepository, dbContextContainer)
        {
        }

        [HttpPost]
        public async Task<IActionResult> GetCousersAsync(IFormFile fileCSV)
        {
            if (fileCSV != null)
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var sreader = ExcelReaderFactory.CreateReader(fileCSV.OpenReadStream()))
                {
                    do
                    {
                        while (sreader.Read()) //Each ROW
                        {
                            for (int column = 0; column < sreader.FieldCount; column++)
                            {
                                //Console.WriteLine(reader.GetString(column));//Will blow up if the value is decimal etc. 
                                Console.WriteLine(sreader.GetValue(column));//Get Value returns object
                            }
                        }
                    } while (sreader.NextResult());
                }
            }
            return Ok();
        }
    }
}
