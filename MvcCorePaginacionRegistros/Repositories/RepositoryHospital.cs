using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;
using System.Data;

#region
/*
CREATE OR ALTER VIEW V_DEPARTAMENTOS_INDIVIDUAL
AS
	SELECT CAST(ROW_NUMBER() OVER (ORDER BY DEPT_NO) AS int) AS POSICION,
	ISNULL(DEPT_NO, 0) AS DEPT_NO, DNOMBRE, LOC
	FROM DEPT
GO

CREATE OR ALTER PROCEDURE SP_GRUPO_DEPARTAMENTOS
(@POSICION INT)
AS
	SELECT DEPT_NO, DNOMBRE, LOC
	FROM V_DEPARTAMENTOS_INDIVIDUAL
	WHERE POSICION >= @POSICION
	AND POSICION < (@POSICION + 2)
GO

CREATE OR ALTER VIEW V_GRUPO_EMPLEADOS
AS
	SELECT CAST(ROW_NUMBER() OVER (ORDER BY EMP_NO) AS int) AS POSICION,
	ISNULL(EMP_NO, 0) AS EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
	FROM EMP
GO

CREATE OR ALTER PROCEDURE SP_GRUPO_EMPLEADOS
(@POSICION INT)
AS
	SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
	FROM V_GRUPO_EMPLEADOS
	WHERE POSICION >= @POSICION
	AND POSICION < (@POSICION + 3)
GO

CREATE OR ALTER PROCEDURE SP_GRUPO_EMPLEADOS_OFICIO
(@POSICION INT, @OFICIO NVARCHAR(100))
AS
	SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
	FROM
	(
		SELECT CAST(ROW_NUMBER() OVER (ORDER BY EMP_NO) AS int) AS POSICION,
		ISNULL(EMP_NO, 0) AS EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
		FROM EMP
		WHERE OFICIO = @OFICIO
	)
	AS QUERY
	WHERE QUERY.POSICION >= @POSICION
	AND QUERY.POSICION < (@POSICION + 2)
GO

CREATE OR ALTER PROCEDURE SP_GRUPO_EMPLEADOS_DEPT_OUT
(@POSICION INT, @DEPT_NO INT, @REGISTROS INT OUT)
AS
	SELECT @REGISTROS = COUNT(EMP_NO)
	FROM EMP
	WHERE DEPT_NO = @DEPT_NO
	SELECT EMP_NO, APELLIDO, SALARIO, OFICIO, DEPT_NO
	FROM
	(
		SELECT CAST(ROW_NUMBER() OVER (ORDER BY DEPT_NO) AS INT) AS POSICION,
		ISNULL(EMP_NO, 0) AS EMP_NO, APELLIDO, SALARIO, OFICIO, DEPT_NO
		FROM EMP
		WHERE DEPT_NO = @DEPT_NO
	)
	AS QUERY
	WHERE QUERY.POSICION = @POSICION
GO
*/
#endregion

namespace MvcCorePaginacionRegistros.Repositories
{
    public class RepositoryHospital
    {
        private HospitalContext context;

        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<Departamento>>
            GetDepartamentosAsync()
        {
            return await this.context.Departamentos.ToListAsync();
        }

        public async Task<ModelPaginacionDepartamentos>
            GetDatosPaginacionDepartAsync(int posicion, int iddepart)
        {
            ModelPaginacionDepartamentos model = new ModelPaginacionDepartamentos();
            model.Departamento = await this.context.Departamentos
                .FirstOrDefaultAsync(d => d.DeptNo == iddepart);
            string sql = "SP_GRUPO_EMPLEADOS_DEPT_OUT @POSICION, @DEPT_NO," +
                "@REGISTROS OUT";
            SqlParameter paramPosicion = new SqlParameter("@POSICION", posicion);
            SqlParameter paramDeptno = new SqlParameter("@DEPT_NO", iddepart);
            SqlParameter paramRegistros = new SqlParameter("@REGISTROS", -1);
            paramRegistros.Direction = ParameterDirection.Output;
            var consulta = this.context.Empleados.FromSqlRaw
                (sql, paramPosicion, paramDeptno, paramRegistros);
            List<Empleado> empleados = await consulta.ToListAsync();
            model.Empleado = empleados.FirstOrDefault();
            model.NumRegistros = (int)paramRegistros.Value;
            return model;
        }


        // El controller nos va a dar una posición y un oficio
        // Debemos devolver empleados y numRegistros
        public async Task<ModelPaginacionEmpleados>
            GetGrupoEmpleadosOficioOutAsync(int posicion, string oficio)
        {
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO_OUT @POSICION, @OFICIO," +
                " @REGISTROS OUT";
            SqlParameter paramPosicion = new SqlParameter("@POSICION", posicion);
            SqlParameter paramOficio = new SqlParameter("@OFICIO", oficio);
            SqlParameter paramRegistros = new SqlParameter("REGISTROS", -1);
            paramRegistros.Direction = ParameterDirection.Output;
            var consulta = this.context.Empleados.FromSqlRaw
                (sql, paramPosicion, paramOficio, paramRegistros);
            List<Empleado> empleados = await consulta.ToListAsync();
            int registros = (int)paramRegistros.Value;
            return new ModelPaginacionEmpleados
            {
                NumeroRegistros = registros,
                Empleados = empleados
            };
        }

        public async Task<int> GetNumeroEmpleadosOficioAsync
            (string oficio)
        {
            return await this.context.Empleados
                .Where(e => e.Oficio == oficio)
                .CountAsync();
        }

        public async Task<List<Empleado>> GetGruposEmpleadosOficioAsync
            (int posicion, string oficio)
        {
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO @POSICION, @OFICIO";
            SqlParameter paramPosicion = new SqlParameter("@POSICION", posicion);
            SqlParameter paramOficio = new SqlParameter("@OFICIO", oficio);
            var consulta = this.context.Empleados
                .FromSqlRaw(sql, paramPosicion, paramOficio);
            return await consulta.ToListAsync();
        }

        public async Task<List<Empleado>> GetGrupoEmpleadosAsync
            (int posicion)
        {
            string sql = "SP_GRUPO_EMPLEADOS @POSICION";
            SqlParameter paramPosicion = new SqlParameter("@POSICION", posicion);
            var consulta = this.context.Empleados.FromSqlRaw(sql, paramPosicion);
            return await consulta.ToListAsync();
        }

        public async Task<int> GetNumEmpleados()
        {
            return await this.context.Empleados.CountAsync();
        }

        public async Task<List<Departamento>>
            GetGrupoDepartamentosAsync(int posicion)
        {
            string sql = "SP_GRUPO_DEPARTAMENTOS @POSICION";
            SqlParameter paramPosicion = new SqlParameter("@POSICION", posicion);
            var consulta = this.context.Departamentos.FromSqlRaw(sql, paramPosicion);
            return await consulta.ToListAsync();
        }

        public async Task<int> GetNumVistaDepartamentos()
        {
            return await this.context.VistaDepartamentos.CountAsync();
        }

        public async Task<VistaDepartamento>
            GetVistaDepartamentoAsync(int posicion)
        {
            VistaDepartamento vista = await this.context.VistaDepartamentos
                .FirstOrDefaultAsync(vd => vd.Posicion == posicion);
            return vista;
        }

        public async Task<List<VistaDepartamento>>
            GetGrupoVistaDepartamentoAsync(int posicion)
        {
            var consulta = from datos in this.context.VistaDepartamentos
                           where datos.Posicion >= posicion
                           && datos.Posicion < (posicion + 2)
                           select datos;
            return await consulta.ToListAsync();
        }

        public async Task<List<Empleado>> GetEmpleadosDepartamenoAsync
            (int iddepart)
        {
            var empleados = this.context.Empleados
                .Where(e => e.IdDepartamento == iddepart);
            if (empleados.Count() == 0) return null;
            return await empleados.ToListAsync();
        }
    }
}
