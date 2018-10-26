using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LogQuake.API.AutoMapper;
using LogQuake.API.ViewModels;
using LogQuake.Domain.Entities;
using LogQuake.Infra.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LogQuake.API.Controllers
{
    public class PlayersController : Controller
    {
        private readonly PlayerRepository _playerRepository;// = new PlayerRepository();

        public PlayersController(PlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        // GET: Players
        public ActionResult Index()
        {
            var playerViewmodel = new object();// Mapper.Map<IEnumerable<Player>, IEnumerable<PlayerViewModel>>(_playerRepository.GetAll());
            return View(playerViewmodel);
        }

        // GET: Players/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Players/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Players/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PlayerViewModel player)
        {
            if(ModelState.IsValid)
            {
                var playerDomain = Mapper.Map<PlayerViewModel, Player>(player);
                _playerRepository.Add(playerDomain);

                return RedirectToAction("Index");
            }

            return View(player);
        }

        // GET: Players/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Players/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Players/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Players/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}