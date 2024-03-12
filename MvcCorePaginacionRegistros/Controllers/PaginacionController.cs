using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.Controllers
{
    public class PaginacionController : Controller
    {
        private RepositoryHospital repo;

        public PaginacionController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> EmpleadosOficioOut
            (int? posicion, string oficio)
        {
            if (posicion == null)
            {
                posicion = 1;
                return View();
            }
            else
            {
                ModelPaginacionEmpleados model = await
                    this.repo.GetGrupoEmpleadosOficioOutAsync(posicion.Value, oficio);
                int registros = await this.repo.GetNumeroEmpleadosOficioAsync(oficio);
                ViewData["REGISTROS"] = model.NumeroRegistros;
                ViewData["OFICIO"] = oficio;
                return View(model.Empleados);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EmpleadosOficioOut
            (string oficio)
        {
            // Cuando buscamos, normalmente, en que posición comienza todo
            ModelPaginacionEmpleados model = await
                    this.repo.GetGrupoEmpleadosOficioOutAsync(1, oficio);
            int registros = await this.repo.GetNumeroEmpleadosOficioAsync(oficio);
            ViewData["REGISTROS"] = model.NumeroRegistros;
            ViewData["OFICIO"] = oficio;
            return View(model.Empleados);
        }

        public async Task<IActionResult> EmpleadosOficio
            (int? posicion, string oficio)
        {
            if (posicion == null)
            {
                posicion = 1;
                return View();
            }
            else
            {
                List<Empleado> empleados = await
                    this.repo.GetGruposEmpleadosOficioAsync(posicion.Value, oficio);
                int registros = await this.repo.GetNumeroEmpleadosOficioAsync(oficio);
                ViewData["REGISTROS"] = registros;
                ViewData["OFICIO"] = oficio;
                return View(empleados);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EmpleadosOficio
            (string oficio)
        {
            // Cuando buscamos, normalmente, en que posición comienza todo
            List<Empleado> empleados =
                await this.repo.GetGruposEmpleadosOficioAsync(1, oficio);
            int registros = await this.repo.GetNumeroEmpleadosOficioAsync(oficio);
            ViewData["REGISTROS"] = registros;
            ViewData["OFICIO"] = oficio;
            return View(empleados);
        }

        public async Task<IActionResult>
            PaginarGrupoVistaDepartamento(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            int numeroRegistros = await
                this.repo.GetNumVistaDepartamentos();
            ViewData["REGISTROS"] = numeroRegistros;
            List<VistaDepartamento> departamentos =
                await this.repo.GetGrupoVistaDepartamentoAsync(posicion.Value);
            return View(departamentos);
        }

        public async Task<IActionResult>
            PaginarGrupoDepartamento(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            int numeroRegistros = await
                this.repo.GetNumVistaDepartamentos();
            ViewData["REGISTROS"] = numeroRegistros;
            List<Departamento> departamentos =
                await this.repo.GetGrupoDepartamentosAsync(posicion.Value);
            return View(departamentos);
        }

        public async Task<IActionResult>
            PaginarGrupoEmpleados(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            int numRegistros = await this.repo.GetNumEmpleados();
            ViewData["REGISTROS"] = numRegistros;
            List<Empleado> empleados =
                await this.repo.GetGrupoEmpleadosAsync(posicion.Value);
            return View(empleados);
        }

        public async Task<IActionResult>
            PaginarRegistroVistaDepartamento(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            int numeroRegistros =
                await this.repo.GetNumVistaDepartamentos();
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
            VistaDepartamento vista =
                await this.repo.GetVistaDepartamentoAsync(posicion.Value);
            ViewData["ÚLTIMO"] = numeroRegistros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            return View(vista);
        }
    }
}
