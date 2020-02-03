﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JLSConsoleApplication.Resources;
using JLSDataAccess.Interfaces;
using JLSDataModel.Models;
using JLSDataModel.Models.Product;
using JLSDataModel.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace JLSMobileApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductController(IProductRepository productRepository, IMapper mapper)
        {
            this._productRepository = productRepository;
            _mapper = mapper;
        }

        [HttpPost("save")]
        public async Task<JsonResult> SaveProduct([FromForm]IFormCollection productData)
        {
            StringValues productInfo;
            StringValues langLabelInfo;
            List<IFormFile> images = (List<IFormFile>) productData.Files;
           
            productData.TryGetValue("product", out productInfo);
            productData.TryGetValue("langLabel", out langLabelInfo);

            ProductRegistrationView productParam = JsonConvert.DeserializeObject<ProductRegistrationView>(productInfo);
            List<ReferenceLabel> langLabels = JsonConvert.DeserializeObject<List<ReferenceLabel>>(langLabelInfo);
            
            Product product = _mapper.Map<Product>(productParam);

            product.ReferenceItem = new ReferenceItem
            {
                Code = productParam.ProductReferenceCode,
                Value = productParam.Name,
                ReferenceCategoryId = productParam.Category
            };

            int res = await this._productRepository.saveProduct(product, images);
            ApiResult result = new ApiResult() { Success = true, Msg = "OK", Type = "200" };
            return Json(result);
        }

        [HttpGet("category")]
        public async Task<JsonResult> GetProductCategory(string lang)
        {
            ApiResult result;
            try
            {
                List<ReferenceItemViewModel> data = await _productRepository.GetProductCategory(lang);
                result = new ApiResult() { Success = true, Msg = "OK", Type = "200", Data = data};
            }
            catch (Exception e)
            {
                result = new ApiResult() { Success = false, Msg = e.Message, Type = "500"};
            }

            return Json(result);
        }

        [HttpGet("taxRate")]
        public async Task<JsonResult> GetTaxRate()
        {
            ApiResult result;
            try
            {
                List<ReferenceItemViewModel> data = await _productRepository.GetTaxRate();
                result = new ApiResult() { Success = true, Msg = "OK", Type = "200", Data = data };
            }
            catch (Exception e)
            {
                result = new ApiResult() { Success = false, Msg = e.Message, Type = "500" };
            }
            return Json(result);
        }
    

    }
}