﻿using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace CRUDExample.Controllers
{
    [Route("[controller]")]
    public class CountriesController : Controller
    {
        private readonly ICountriesService _countriesService;
        public CountriesController(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }



        [Route("[action]")]
        public IActionResult UploadFromExcel()
        {
            return View();
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
        {
            if(excelFile == null || excelFile.Length == 0)
            {
                ViewBag.ErrorMessage = "Please select an xlsx file";
                return View();
            }

            if(!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Usupported file. 'xlsx' file is expected";
                return View();
            }

            int countriesCountInserted = await _countriesService.UploadCountriesFromExcel(excelFile);
            ViewBag.Message = $"{countriesCountInserted} Countries Upload";
            return View();
        }
    }
}
