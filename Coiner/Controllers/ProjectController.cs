using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Coiner.Business.Models;
using Coiner.Business.Services;
using Coiner.Business.Models.Enums;
using Coiner.Controllers.ModelsDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;
using log4net;
using System.Reflection;
using Coiner.Business.LoggerService;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Coiner.Business.Heplers;
using MangoPay.SDK.Entities.GET;

namespace Coiner.Controllers
{
    [Produces("application/json")]
    [Route("api/Projects")]
    public class ProjectController : Controller
    {


        ProjectService _projectService = new ProjectService();
        BlockChainService _blockChainService = new BlockChainService();
        MangoPayService _mangoPayService = new MangoPayService();

        private static readonly ILog Log = LogManager.GetLogger(typeof(ProjectController));

        [Authorize]
        [HttpPost]
        [Route("CreateProject")]
        public IActionResult CreateProject([FromBody] Project project)
        {
            try
            {
                if (_projectService.CreateProject(project))
                {
                    return Ok();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("GetProjects")]
        public IActionResult GetProjects()
        {
            try
            {
                var projects = _projectService.GetProjects();
                return Ok(projects);
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("GetLatestProjects")]
        public IActionResult GetLatestProjects()
        {
            try
            {
                var latestProjects = _projectService.GetLatestProjects();
                var allProjectsCount = _projectService.GetAllProjectsCount();
                return Ok(new object[] {
                    latestProjects,
                    allProjectsCount
                });
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("GetLatestProducts")]
        public async Task<IActionResult> GetLatestProducts()
        {
            try
            {
                var latestProducts = await _blockChainService.GetListProducts();
                var allProductsCount = await _blockChainService.GetAllProductsCount();
                return Ok(new object[] {
                    latestProducts.Take(9).ToList(),
                    allProductsCount
                });
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }

        }

        [HttpPost]
        [Route("GetFilteredProjects")]
        public IActionResult GetFilteredProjects([FromBody] ProjectFilterDto projectFilterDto)
        {
            try
            {
                var projects = _projectService.GetFilteredProjects(projectFilterDto.ProjectActivityTypes,
                                                                   projectFilterDto.ProjectTypes,
                                                                   projectFilterDto.PageIndex,
                                                                   projectFilterDto.PageSize);
                var projectsCount = _projectService.GetFilteredProjectsCount(projectFilterDto.ProjectActivityTypes,
                                                                             projectFilterDto.ProjectTypes);
                return Ok(new { projects, projectsCount });
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpPost]
        [Route("GetFilteredProducts")]
        public async Task<IActionResult> GetFilteredProducts([FromBody] ProductFilterDto productFilterDto)
        {
            try
            {
                var products = await _blockChainService.GetFilteredProducts(productFilterDto.PageIndex,
                                                                      productFilterDto.PageSize);
                var productsCount = await _blockChainService.GetAllProductsCount();
                return Ok(new { products, productsCount });
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("GetUserProjects/{userId}/{pageIndex}/{pageSize}")]
        public IActionResult GetUserProjects(int userId, int pageIndex, int pageSize)
        {
            try
            {
                var projects = _projectService.GetUserProjects(userId, pageIndex, pageSize);
                var projectsCount = _projectService.getUserProjectsCount(userId);
                return Ok(new { projects, projectsCount });

            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("AddCoinsToProject")]
        public async Task<IActionResult> AddCoinsToProject([FromBody] AddCoinsDto addCoinsDto)
        {
            try
            {
                var investmentDone = await _mangoPayService.GetInvestmentAmount(addCoinsDto.TransactionId, addCoinsDto.UserWallerID);
                if (investmentDone)
                {
                    var data = _projectService.AddCoinsToProject(addCoinsDto.Coin);
                    return Ok(data);
                }
                else
                {
                    return new StatusCodeResult(503);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("SearchForProjects/{searchInput}")]
        public IActionResult SearchForProjects(string searchInput)
        {
            try
            {
                var projects = _projectService.SearchForProjects(searchInput);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("FetchProjectImagesContent/{projectId}")]
        public IActionResult FetchProjectImagesContent(int projectId)
        {
            try
            {
                var projectImages = _projectService.FetchImagesContent(projectId);
                return Ok(projectImages);
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("FetchProjectDocumentsContent/{projectId}")]
        public IActionResult FetchProjectDocumentsContent(int projectId)
        {
            try
            {
                var projectDocuments = _projectService.FetchDocumentsContent(projectId);
                return Ok(projectDocuments);
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }

        }

        [Authorize]
        [HttpPost]
        [Route("UpdateProject")]
        public IActionResult UpdateProject([FromBody]Project project)
        {
            try
            {
                if (_projectService.UpdateProject(project))
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("AddNewsToProject")]
        public IActionResult AddNewsToProject([FromBody] ProjectUpdate projectUpdate)
        {
            try
            {
                if (_projectService.AddNewsToProject(projectUpdate))
                {
                    return Ok();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpPost]
        [Route("GetFavoriteProjects/")]
        public IActionResult GetFavoriteProjects([FromBody]List<int> ids)
        {
            try
            {
                var favoriteProjects = _projectService.GetFavoriteProjects(ids);
                return Ok(favoriteProjects);

            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("GetUserCoinsForProject/{userId}/{projectId}")]
        public IActionResult GetUserCoinsForProject(int userId, int projectId)
        {
            try
            {
                var coinsNumber = _projectService.GetUserCoinsForProject(userId, projectId);
                return Ok(coinsNumber);
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }

        }

        [Authorize]
        [HttpPost]
        [Route("AddQuestionToProject")]
        public IActionResult AddQuestionToProject([FromBody] Discussion discussion)
        {
            try
            {
                var discussions = _projectService.AddQuestionToProject(discussion);
                if (discussions != null)
                {
                    return Ok(discussions);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("AddAnswerToQuestion")]
        public IActionResult AddAnswerToQuestion([FromBody] Discussion discussion)
        {
            try
            {
                var discussions = _projectService.AddAnswerToQuestion(discussion);
                if (discussions != null)
                {
                    return Ok(discussions);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("GetLatestDummyProducts")]
        public IActionResult GetLatestDummyProducts()
        {
            try
            {
                var latestProducts = _projectService.GetDummyProduts().Take(9).ToList();
                var allProductsCount = _projectService.GetAllDummyProductsCount();
                return Ok(new object[] {
                    latestProducts,
                    allProductsCount
                });
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }

        }

        [HttpGet]
        [Route("GetProjectId/{projectId}")]
        public IActionResult GetProjectId(int projectId)
        {
            try
            {
                var projectUrl = _projectService.GetProjectId(projectId);
                return Ok(projectUrl);
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("EndSurvey/{projectId}")]
        public async Task<IActionResult> EndSurvey(int projectId)
        {
            try
            {
                var project = await _projectService.EndSurvey(projectId);
                return Ok(project);
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("GetProductFromUrl/{productName}")]
        public async Task<IActionResult> GetProductFromUrl(string productName)
        {
            try
            {
                var productUrl = await _blockChainService.GetProductFromUrl(productName);
                return Ok(productUrl);
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }
        [HttpGet]
        [Route("GetUserAssetBalance/{productName}/{userId}")]
        public async Task<IActionResult> GetUserAssetBalance(string productName,int userId)
        {
            try
            {
                
                var userAssetBalance = await _blockChainService.GetUserProductAssetBalance(productName, userId);
                return Ok(userAssetBalance);
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpPost]
        [Route("SendToCreateBlockChainOffer")]
        public async Task<IActionResult> SendToCreateBlockChainOffer([FromBody] BlockChainOfferDto blockChainOfferDto)
        {
            try
            {
                var productOffers = await _blockChainService.CreateNewOffer(blockChainOfferDto.UserId,
                                                  blockChainOfferDto.PrivateKey,
                                                  blockChainOfferDto.ProductName,
                                                  blockChainOfferDto.ProductQuantity,
                                                  blockChainOfferDto.Currency,
                                                  blockChainOfferDto.CurrencyQuantity,
                                                  blockChainOfferDto.OfferType,
                                                  blockChainOfferDto.CommissionFees);
                return Ok(productOffers);
            }
            catch (LucidOcean.MultiChain.Exceptions.JsonRpcException ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                var error = new
                {
                    ErrorCode = ex.Error.Code,
                    ErrorMessage = ex.Error.Message
                };
                return BadRequest(error);

            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("GetProductOffersFromStreams/{productName}")]
        public async Task<IActionResult> GetProductOffersFromStreams(string productName)
        {
            try
            {
                var offers = await _blockChainService.GetProductOffersFromStreams(productName);
                return Ok(offers);
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("confirmProject/{projectId}")]
        public IActionResult confirmProject(int projectId)
        {
            try
            {
                if (_projectService.confirmProject(projectId))
                {
                    return Ok();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpPost]
        [Route("AcceptOffer")]
        public async Task<IActionResult> AcceptOffer([FromBody] OfferDto offerDto, string zoom)
        {
            try
            {
                var data = await _blockChainService.AcceptOffer(offerDto.UserId,
                                                offerDto.ProductName,
                                                offerDto.PrivateKey,
                                                offerDto.ProductOffer.AskAssetName,
                                                offerDto.ProductOffer.AskQuantity,
                                                offerDto.ProductOffer.OfferAssetName,
                                                offerDto.ProductOffer.OfferQuantity,
                                                offerDto.ProductOffer.HexBlob,
                                                offerDto.ProductOffer.PxCoin,
                                                offerDto.ProductOffer.TxId,
                                                offerDto.ProductOffer.FromAddress,
                                                zoom,
                                                offerDto.CommissionFees);




                return Ok(data);
            }
            catch (LucidOcean.MultiChain.Exceptions.JsonRpcException ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                var error = new
                {
                    ErrorCode = ex.Error.Code,
                    ErrorMessage = ex.Error.Message
                };
                return BadRequest(error);

            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpPost]
        [Route("CancelOffer")]
        public async Task<IActionResult> CancelOffer([FromBody] OfferDto offerDto)
        {
            try
            {
                var produtOffers = await _blockChainService.CancelOffer(offerDto.UserId,
                                                offerDto.ProductName,
                                                offerDto.PrivateKey,
                                                offerDto.ProductOffer.AskAssetName,
                                                offerDto.ProductOffer.AskQuantity,
                                                offerDto.ProductOffer.OfferAssetName,
                                                offerDto.ProductOffer.OfferQuantity,
                                                offerDto.ProductOffer.HexBlob,
                                                offerDto.ProductOffer.PxCoin,
                                                offerDto.ProductOffer.TxId);
                return Ok(produtOffers);
            }
            catch (LucidOcean.MultiChain.Exceptions.JsonRpcException ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                var error = new
                {
                    ErrorCode = ex.Error.Code,
                    ErrorMessage = ex.Error.Message
                };
                return BadRequest(error);

            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpPost]
        [Route("CreditCurrency")]
        public async Task<IActionResult> CreditCurrency([FromBody] CreditCurrencyDto creditCurrencyDto)
        {
            try
            {
                var payInObject = await _mangoPayService.CreateCashInCryptoCurrency(creditCurrencyDto.WalletId, creditCurrencyDto.CurrencyQuantity, creditCurrencyDto.ProjectId, creditCurrencyDto.RedirectPage);
                return Ok(payInObject);
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpPost]
        [Route("InvestCurrency")]
        public async Task<IActionResult> InvestCurrency([FromBody] InvestCurrencyDto investCurrencyDto)
        {
            try
            {
                var payInObject = await _mangoPayService.CreateCashInInvestment(investCurrencyDto.WalletId, investCurrencyDto.CurrencyQuantity, Convert.ToInt32(investCurrencyDto.ProjectId));
                return Ok(payInObject);
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("CreditCryptoCurrency/{userId}/{transactionId}")]
        public async Task<IActionResult> CreditCryptoCurrency(int userId, string transactionId)
        {
            try
            {
                var currencyQuantity = await _mangoPayService.GetCryptoCurrencyAmount(transactionId);
                User currentUser = null;

                if (currencyQuantity != 0)
                {
                    currentUser = await _blockChainService.CreditCurrency(userId, currencyQuantity);
                    return Ok(currentUser);
                }
                else
                {
                    return new StatusCodeResult(503);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("UpdateStats/{productName}/{zoom}")]
        public async Task<IActionResult> UpdateStats(string productName, string zoom)
        {
            try
            {
                var stats = await _blockChainService.GetStatsForProduct(productName, zoom);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }

        }

        [HttpGet]
        [Route("GetUserProductsBuyList/{userId}")]
        public async Task<IActionResult> GetUserProductsBuyList(int userId)
        {
            try
            {
                var productbuyoffers = await _blockChainService.GetUserProductsBuyList(userId);
                return Ok(productbuyoffers);
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("GetUserEuroBalance/{userId}")]
        public async Task<IActionResult> GetUserEuroBalance(int userId)
        {
            try
            {
                var Balance = await _blockChainService.GetUserEuroBalance(userId);
                return Ok(Balance);
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("GetUserStatsDetails/{userId}")]
        public async Task<IActionResult> GetUserStatsDetails(int userId)
        {
            try
            {
                var UserStatsDetailsList = await _blockChainService.GetUserStatsDetails(userId);
                return Ok(UserStatsDetailsList);
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpPost]
        [Route("UploadFroalaImages")]
        public async Task<IActionResult> UploadFroalaImages()
        {
            try
            {
                return Ok(await _projectService.AddFroalaImages(HttpContext));
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpPost]
        [Route("UploadFroalaVideos")]
        public async Task<IActionResult> UploadFroalaVideos()
        {
            try
            {
                return Ok(await _projectService.AddFroalaVideos(HttpContext));
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpPost]
        [Route("UploadFroalaFiles")]
        public async Task<IActionResult> UploadFroalaFiles()
        {
            try
            {
                return Ok(await _projectService.AddFroalaFiles(HttpContext));
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpPost]
        [Route("RemoveFroalaImages")]
        public IActionResult RemoveFroalaImages([FromBody] List<string> imgname)
        {
            try
            {
                return Ok(_projectService.RemoveFroalaImages(imgname));
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpPost]
        [Route("RemoveFroalaVideos")]
        public IActionResult RemoveFroalaVideos([FromBody] List<string> videosName)
        {
            try
            {
                return Ok(_projectService.RemoveFroalaVideos(videosName));
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpPost]
        [Route("RemoveFroalaFiles")]
        public IActionResult RemoveFroalaFiles([FromBody] List<string> filesName)
        {
            try
            {
                return Ok(_projectService.RemoveFroalaFiles(filesName));
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("GetUserBills/{userId}")]
        public IActionResult GetUserBills(int userId)
        {
            try
            {
                var bills = _projectService.GetUserBills(userId);
                return Ok(bills);
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpPost]
        [Route("payOut")]
        public async Task<IActionResult> PayOut([FromBody] PayOutEncaseDto payOutEncaseDto)
        {
            try
            {
                PayOutBankWireDTO result = null;
                var cryptoCredited = await _blockChainService.SendCryptoToBurnAddress(payOutEncaseDto.UserId, payOutEncaseDto.PrivateKey, payOutEncaseDto.DebitedAmount);
                if (cryptoCredited)
                {
                    result = await _mangoPayService.CreatePayOut(payOutEncaseDto.UserId, payOutEncaseDto.DebitedAmount);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpPost]
        [Route("investWithCryptoEuro")]
        public async Task<IActionResult> investWithCryptoEuro([FromBody] InvestWithCryptoEuroDto investWithCryptoEuroDto)
        {
            try
            {
                //var CoinsAddedStatus = _projectService.ChekCoinsAdded(investWithCryptoEuroDto.Coin);
                //if (CoinsAddedStatus)
                //{
                var BlockChainInvestStatus = await _blockChainService.SendCryptoToBurnAddress(investWithCryptoEuroDto.UserId, investWithCryptoEuroDto.PrivateKey, investWithCryptoEuroDto.CryptoEuroQuantity);

                if (BlockChainInvestStatus)
                {
                    var MangoPayInvestStatus = await _mangoPayService.SendMoneyFromContonmentToOwnerProject(investWithCryptoEuroDto.OwnerProjectWalletId, investWithCryptoEuroDto.CryptoEuroQuantity, 0);
                    if (MangoPayInvestStatus)
                    {
                        var data = _projectService.AddCoinsToProject(investWithCryptoEuroDto.Coin);
                        return Ok(data);
                    }
                    else
                    {
                        return new StatusCodeResult(503);
                    }
                }
                else
                {
                    return new StatusCodeResult(503);
                }
                //}
                //else
                //{
                //    return new StatusCodeResult(503);
                //}
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

        [HttpGet]
        [Route("RefundUsers/{projectId}")]
        public async Task<IActionResult> RefundUsers(int projectId)
        {
            try
            {
                var project = await _projectService.RefundUsers(projectId);
                if (project != null)
                {
                    return Ok(project);
                }
                else
                {
                    return new StatusCodeResult(503);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(Log, ex, MethodBase.GetCurrentMethod());
                return new StatusCodeResult(503);
            }
        }

    }
}
