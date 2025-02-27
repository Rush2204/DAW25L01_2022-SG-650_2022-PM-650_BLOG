﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using L01_NUMEROS_CARNETS.Models;

namespace L01_NUMEROS_CARNETS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class usuariosController : Controller
    {
        private readonly BlogDBContext _BlogDBContext;

        public usuariosController(BlogDBContext BlogDBContext)
        {
            _BlogDBContext = BlogDBContext;
        }

        // Para poder ver todos los registros:

        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<usuarios> listadoUsuario = (from e in _BlogDBContext.usuarios select e).ToList();

            if (listadoUsuario.Count == 0)
            {
                return NotFound();
            }
            return Ok(listadoUsuario);
        }

        // Para agregar un nuevo registro:

        [HttpPost]
        [Route("Add")]

        public IActionResult GuardarUsuario([FromBody] usuarios usuario)
        {
            try
            {
                _BlogDBContext.usuarios.Add(usuario);
                _BlogDBContext.SaveChanges();
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Para actualizar un registro

        [HttpPut]
        [Route("actualizar/{id}")]

        public IActionResult ActualizarUsuario(int id, [FromBody] usuarios usuarioModificar)
        {
            usuarios? usuarioActual = (from e in _BlogDBContext.usuarios where e.usuarioId == id select e).FirstOrDefault();

            if (usuarioActual == null)
            { return NotFound(); }

            usuarioActual.nombreUsuario = usuarioModificar.nombreUsuario;
            usuarioActual.clave = usuarioModificar.clave;
            usuarioActual.nombre = usuarioModificar.nombre;
            usuarioActual.apellido = usuarioModificar.apellido;

            _BlogDBContext.Entry(usuarioActual).State = EntityState.Modified;
            _BlogDBContext.SaveChanges();

            return Ok(usuarioModificar);
        }

        // Para Eliminar un registro

        [HttpDelete]
        [Route("eliminar/{id}")]

        public IActionResult EliminarUsuario(int id)
        {
            usuarios? usuario = (from e in _BlogDBContext.usuarios where e.usuarioId == id select e).FirstOrDefault();

            if (usuario == null)
            { return NotFound(); }

            _BlogDBContext.usuarios.Attach(usuario);
            _BlogDBContext.usuarios.Remove(usuario);
            _BlogDBContext.SaveChanges();

            return Ok(usuario);
        }

        //Retornar el listado por rol

        [HttpGet]
        [Route("GetByRol/{rolId}")]

        public IActionResult GetRol(int rolId)
        {
            var usuarios = (from e in _BlogDBContext.usuarios
                            where e.rolId == rolId
                                  select new
                                  {
                                      e.usuarioId,
                                      e.rolId,
                                      e.nombreUsuario,
                                      e.nombre,
                                      e.apellido,
                                  }).ToList();

            if (usuarios == null || usuarios.Count ==0)
            {
                return NotFound();
            }
            return Ok(usuarios);
        }

        //Retornar el listado por nombre y apellido

        [HttpGet]
        [Route("GetByNyA/{nombre}/{apellido}")]

        public IActionResult GetNyA(string nombre, string apellido)
        {
            var usuarios = (from e in _BlogDBContext.usuarios
                            where e.nombre == nombre && e.apellido == apellido
                            select new
                            {
                                e.usuarioId,
                                e.rolId,
                                e.nombreUsuario,
                                e.nombre,
                                e.apellido,
                            }).ToList();

            if (usuarios == null || usuarios.Count == 0)
            {
                return NotFound();
            }
            return Ok(usuarios);
        }

        //Retornar el listado de top N 

        [HttpGet]
        [Route("GetTopNUsuario")]

        public IActionResult Get2()
        {
            var usuario = (from c in _BlogDBContext.comentarios
                           join u in _BlogDBContext.usuarios
                           on c.usuarioId equals u.usuarioId
                           group c by new { u.usuarioId, u.nombreUsuario, u.nombre, u.apellido } into g
                           orderby g.Count() descending
                           select new
                           {
                               g.Key.usuarioId,
                               g.Key.nombreUsuario,
                               g.Key.nombre,
                               g.Key.apellido,
                               cantidadComentarios = g.Count()
                           }).Take(3).ToList();

            if (usuario == null || !usuario.Any())
            {
                return NotFound();
            }
            return Ok(usuario);
        }
    }
}
