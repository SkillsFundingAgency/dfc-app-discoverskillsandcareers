using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Data;
using DFC.Compui.Sessionstate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class BaseController<TController> : Controller
    {
        public BaseController(ILogger<TController> logger, ISessionStateService<SessionDataModel> sessionStateService)
        {
            Logger = logger;
            SessionStateService = sessionStateService;
        }

        protected ILogger<TController> Logger { get; private set; }

        protected ISessionStateService<SessionDataModel> SessionStateService { get; private set; }

        protected IActionResult RedirectTo(string relativeAddress)
        {
            relativeAddress = $"~/{RouteName.Prefix}/" + relativeAddress;
            return Redirect(relativeAddress);
        }

        protected IActionResult RedirectToRoot()
        {
            return RedirectTo(string.Empty);
        }

        public bool HasSessionId()
        {
            return Request.CompositeSessionId().HasValue;
        }

        protected async Task<SessionStateModel<SessionDataModel>?> GetSessionStateAsync()
        {
            var compositeSessionId = Request.CompositeSessionId();
            if (compositeSessionId.HasValue)
            {
                Logger.LogInformation($"Getting the session state - compositeSessionId = {compositeSessionId}");

                return await SessionStateService.GetAsync(compositeSessionId.Value).ConfigureAwait(false);
            }

            Logger.LogError($"Error getting the session state - compositeSessionId = {compositeSessionId}");

            return default;
        }

        protected async Task<bool> SetSessionStateAsync(string applicationName, string salt = null)
        {
            var compositeSessionId = Request.CompositeSessionId();
            if (compositeSessionId.HasValue)
            {
                Logger.LogInformation($"Getting the session state - compositeSessionId = {compositeSessionId}");

                var sessionStateModel = await SessionStateService.GetAsync(compositeSessionId.Value).ConfigureAwait(false);
                sessionStateModel.Ttl = 43200;
                sessionStateModel.State!.ApplicationName = applicationName;
                sessionStateModel.State!.Salt = salt;

                Logger.LogInformation($"Saving the session state - compositeSessionId = {compositeSessionId}");

                var result = await SessionStateService.SaveAsync(sessionStateModel).ConfigureAwait(false);

                return result == HttpStatusCode.OK || result == HttpStatusCode.Created;
            }

            Logger.LogError($"Error saving the session state - compositeSessionId = {compositeSessionId}");

            return false;
        }

        protected async Task<bool> DeleteSessionStateAsync()
        {
            var compositeSessionId = Request.CompositeSessionId();
            if (compositeSessionId.HasValue)
            {
                Logger.LogInformation($"Deleting the session state - compositeSessionId = {compositeSessionId}");

                return await SessionStateService.DeleteAsync(compositeSessionId.Value).ConfigureAwait(false);
            }

            Logger.LogError($"Error deleting the session state - compositeSessionId = {compositeSessionId}");

            return false;
        }

        protected async Task<string> GetDysacIdAsync()
        {
            var sessionState = await SessionStateService.GetAsync(Request.CompositeSessionId().Value).ConfigureAwait(false);
            return sessionState.State.DysacSessionId;
        }
    }
}
