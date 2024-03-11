using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.ViewComponents
{
    [ViewComponent(Name = "MenuDepart")]
    public class MenuDepartViewController : ViewComponent
    {
        private RepositoryHospital repo;

        public MenuDepartViewController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Departamento> departamentos =
                await this.repo.GetDepartamentosAsync();
            return View(departamentos);
        }
    }
}
