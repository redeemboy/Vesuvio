using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vesuvio.Domain.DTO;
using Vesuvio.Service;
using Vesuvio.WebAPI.Helpers;

namespace Vesuvio.API.Controllers
{

    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private IWebHostEnvironment _webHostEnvironment;
        public string _baseUrl;

        public ProductController(IProductService productService, IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;

            _webHostEnvironment = webHostEnvironment;
            if (string.IsNullOrWhiteSpace(_webHostEnvironment.WebRootPath))
            {
                _webHostEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }
            _baseUrl = _webHostEnvironment.WebRootPath;
        }

        [HttpPost]
        [Route("getall")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetAll(DataTableRequest request)
        {
            DataTableResponse productsResponse = _productService.GetProductsResponse(request);

            if (productsResponse.Data.Count() > 0)
            {
                return Ok(productsResponse);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("getbyid/{Id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetById(Guid Id)
        {
            ProductResponseDTO product = _productService.GetProductById(Id);

            if (product != null)
            {
                return Ok(product);
            }
            else return NotFound();
        }

        [HttpPost]
        [Route("create")]
        [Authorize(Policy = "Admin")]
        public IActionResult Post([FromForm] ProductDTO productDTO)
        {
            var file = Request.Form.Files;
            var fileName = string.Empty;
            if (file.Count > 0)
            {
                string path = Path.Combine(_baseUrl, @"Images\Product");
                fileName = CommonHelper.SaveImage(path, file[0]);
            }

            productDTO.ImageUrl = fileName;
            bool productCreate = _productService.CreateProduct(productDTO);
            if (productCreate) return Ok();
            else return BadRequest();
        }

        [HttpPut]
        [Route("update")]
        [Authorize(Policy = "Admin")]
        public IActionResult Put([FromForm] ProductDTO productDTO)
        {
            var file = Request.Form.Files;
            var fileName = string.Empty;

            string path = Path.Combine(_baseUrl, @"Images\Product");
            if (productDTO.RemoveImage)
            {
                if (!string.IsNullOrEmpty(productDTO.ImageUrl))
                {
                    CommonHelper.RemoveFile(path, productDTO.ImageUrl);
                    productDTO.ImageUrl = null;
                }
            }

            if (file.Count > 0)
            {
                fileName = CommonHelper.SaveImage(path, file[0]);
                productDTO.ImageUrl = fileName;
            }

            bool productUpdate = _productService.UpdateProduct(productDTO);
            if (productUpdate)
            {
                return Ok();
            }
            else return BadRequest();
        }

        [HttpPatch("delete")]
        [Authorize(Policy = "Admin")]
        public IActionResult Delete([FromBody] List<string> ids)
        {
            bool result = _productService.DeleteProducts(ids);

            if (result)
            {
                return Ok();
            }
            else return BadRequest();
        }

    }
}