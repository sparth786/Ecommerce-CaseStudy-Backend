using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using EcommerceApp.Models;
using System.Linq;
using Newtonsoft.Json;
using System;

namespace EcommerceApp
{
    public static class Function1
    {
        [FunctionName("GetCartDataFunction")]
        public static async Task<IActionResult> GetCartData(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "cart")] HttpRequestMessage req, ILogger log, ExecutionContext context)
        {
            try
            {
                log.LogInformation("Getting data from database.");

                var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                using (var dbContext = DbContextHelper.GetDbContext(config))
                {
                    var cart = await dbContext.CartDetails
                        .Join(dbContext.ProductMaster, c => c.ProductId, p => p.Id, (c, p) => new { c, p })
                        .Join(dbContext.UserMaster, c => c.c.UserId, u => u.Id, (cp, u) => new { cp.c, cp.p, u })
                        .Select(x => new CartData
                        {
                            CartId = x.c.Id,
                            ProductName = x.p.Name,
                            UserName = x.u.FullName,
                            ImageURL = x.p.ImageURL,
                            Quantity = x.c.Quantity,
                            Price = x.p.Price,
                            TotalAmount = x.c.TotalAmount,
                        })
                        .ToListAsync();
                    return new OkObjectResult(cart);
                }
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }

        [FunctionName("GetProductDataFunction")]
        public static async Task<IActionResult> GetProductData(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products")] HttpRequestMessage req, ILogger log, ExecutionContext context)
        {
            try
            {
                log.LogInformation("Getting data from database.");

                var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                using (var dbContext = DbContextHelper.GetDbContext(config))
                {
                    List<ProductMaster> products = await dbContext.ProductMaster.Include(x => x.Category).ToListAsync();
                    return new OkObjectResult(products);
                }
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }

        [FunctionName("GetProductDataByIdFunction")]
        public static async Task<IActionResult> GetProductDataById(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "product/{id:int}")] HttpRequestMessage req, int id, ILogger log, ExecutionContext context)
        {
            try
            {
                log.LogInformation("Getting data from database.");

                var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                using (var dbContext = DbContextHelper.GetDbContext(config))
                {
                    ProductMaster product = await dbContext.ProductMaster
                        .Include(x => x.Category)
                        .Where(x => x.Id == id)
                        .FirstOrDefaultAsync();
                    if (product == null)
                    {
                        return new NotFoundResult();
                    }
                    return new OkObjectResult(product);
                }
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }

        [FunctionName("AddToCartFunction")]
        public static async Task<IActionResult> AddToCart(
         [HttpTrigger(AuthorizationLevel.Function, "post", Route = "cart/add")] HttpRequestMessage req,
         ILogger log,
         ExecutionContext context)
        {
            try
            {
                log.LogInformation("Adding product to cart.");

                var config = new ConfigurationBuilder()
                   .SetBasePath(context.FunctionAppDirectory)
                   .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                   .AddEnvironmentVariables()
                   .Build();

                using (var dbContext = DbContextHelper.GetDbContext(config))
                {
                    string requestBody = await req.Content.ReadAsStringAsync();
                    var input = JsonConvert.DeserializeObject<AddToCartVM>(requestBody);

                    var product = await dbContext.ProductMaster.FindAsync(input.ProductId);
                    var user = await dbContext.UserMaster.FindAsync(Convert.ToInt32(input.UserId));

                    if (product == null)
                    {
                        return new BadRequestObjectResult("Invalid ProductId");
                    }

                    var cartDetail = new CartDetails
                    {
                        ProductId = input.ProductId,
                        UserId = input.UserId,
                        Quantity = input.Quantity,
                        TotalAmount = input.Quantity * product.Price,
                        Image = input.ImageURL,
                    };

                    await dbContext.CartDetails.AddAsync(cartDetail);
                    await dbContext.SaveChangesAsync();

                    return new OkObjectResult(cartDetail);
                }
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }

        [FunctionName("DeleteCartDataFunction")]
        public static async Task<IActionResult> DeleteCartData(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "delete/cart/{id:int}")] HttpRequestMessage req, int id, ILogger log, ExecutionContext context)
        {
            try
            {
                log.LogInformation("delete cart id from database.");
                var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                using (var dbContext = DbContextHelper.GetDbContext(config))
                {
                    var cartid = dbContext.CartDetails.Where(x => x.Id == id).FirstOrDefault();

                    dbContext.CartDetails.Remove(cartid);
                    await dbContext.SaveChangesAsync();

                    return new OkObjectResult(cartid);
                }
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }
    }
}
