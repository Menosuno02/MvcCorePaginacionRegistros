using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.Controllers
{
    public class DepartamentosController : Controller
    {
        private RepositoryHospital repo;

        public DepartamentosController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<Departamento> dept = await this.repo.GetDepartamentosAsync();
            return View(dept);
        }

        public async Task<IActionResult> Details
            (int? posicion, int iddepart)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            ModelPaginacionDepartamentos model =
                await this.repo.GetDatosPaginacionDepartAsync
                (posicion.Value, iddepart);
            int numeroRegistros = model.NumRegistros;
            int siguiente = posicion.Value + 1;
            if (siguiente > numeroRegistros)
            {
                siguiente = numeroRegistros;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 1)
            {
                anterior = 1;
            }
            ViewData["ÚLTIMO"] = numeroRegistros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            ViewData["DEPART"] = model.Departamento;
            ViewData["POSICION"] = posicion;
            return View();
        }

        public async Task<IActionResult> _Empleado
            (int? posicion, int iddepart)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            ModelPaginacionDepartamentos model =
                await this.repo.GetDatosPaginacionDepartAsync
                (posicion.Value, iddepart);
            int numeroRegistros = model.NumRegistros;
            int siguiente = posicion.Value + 1;
            if (siguiente > numeroRegistros)
            {
                siguiente = numeroRegistros;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 1)
            {
                anterior = 1;
            }
            ViewData["ÚLTIMO"] = numeroRegistros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            ViewData["DEPART"] = model.Departamento;
            ViewData["POSICION"] = posicion;
            return PartialView("_Empleado", model.Empleado);
        }
    }
}
