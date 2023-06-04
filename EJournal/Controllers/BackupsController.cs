using Aspose.Foundation.UriResolver.RequestResponses;
using Aspose.Pdf;
using EJournal.Models.ViewModels.BackupViewModels;
using EJournal.Response;
using EJournal.Service.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace EJournal.Controllers
{
    public class BackupsController : Controller
    {
        private readonly IDumpService _dumpService;
		public BackupsController(IDumpService dumpService)
        {
			_dumpService = dumpService;

		}
        [Authorize(Policy = WC.PolicyOnlyForAdmin)]
        public async Task<IActionResult> Index()
        {
            BaseResponse<string[]> response = await _dumpService.GetBackUpsName();
            if(response.StatusCode == StatusCodeEnum.OK)
            {
				
				return View(response.Data);
			}
            else
            {
                return NotFound();
            }
            
        }
        [Authorize(Policy = WC.PolicyOnlyForAdmin)]
        public async Task<IActionResult> Create()
		{
            await _dumpService.CreateDump();

            return RedirectToAction(nameof(Index));
		}

        [HttpGet]
        [Authorize(Policy = WC.PolicyOnlyForAdmin)]
        public async Task<IActionResult> Delete(string? nameDump)
		{
            if (nameDump != null)
            {
                await _dumpService.DeleteDump(nameDump);
            }

			return RedirectToAction(nameof(Index));
		}
		[HttpGet]
        [Authorize(Policy = WC.PolicyOnlyForAdmin)]
        public async Task<IActionResult> Jump(string? nameDump)
		{
			
			if (nameDump != null)
			{
				var response = await _dumpService.JumpDump(nameDump);
				if(response.StatusCode == StatusCodeEnum.OK)
				{
					return View(new BackupViewModel(response.Data, false));
				}
				else
				{
					return View(new BackupViewModel(response.Data, true));
				}
			}
			
			return View(new BackupViewModel("", true));
		}
	}
}
