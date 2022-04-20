using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P224Allup.DAL;
using P224Allup.Extensions;
using P224Allup.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P224Allup.Areas.Manage.Controllers
{
    [Area("manage")]
    public class BrandController : Controller
    {
        private readonly AllupDbContext _context;
        public BrandController(AllupDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Brand> brands = await _context.Brands.ToListAsync();
            return View(brands);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Brand brand)
        {
            if (!ModelState.IsValid) return View();
            if (brand == null) return BadRequest();
            if (string.IsNullOrWhiteSpace(brand.Name))
            {
                ModelState.AddModelError("Name", "Bosluq Olmamalidir");
                return View();
            }


            if (brand.Name.CheckString())
            {
                ModelState.AddModelError("Name", "Yalniz Herf Ola Biler");
                return View();
            }

            if (await _context.Brands.AnyAsync(t => t.Name.ToLower() == brand.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Alreade Exists");
                return View();
            }

            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!ModelState.IsValid) return View();
            if (id == null) return BadRequest();
            Brand brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);
            if (brand == null) return NotFound();

            brand.IsDeleted = true;
            brand.DeletedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();

            return PartialView("_BrandIndexPartial", await _context.Brands.ToListAsync());
        }
        public async Task<IActionResult> Restore(int? id)
        {
            if (!ModelState.IsValid) return View();
            if (id == null) return BadRequest();
            Brand brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);
            if (brand == null) return NotFound();

            brand.IsDeleted = false;

            await _context.SaveChangesAsync();

            return PartialView("_BrandIndexPartial",await _context.Brands.ToListAsync());
        }
        public IActionResult Update()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Brand brand)
        {
            if (!ModelState.IsValid) return View();
            Brand dbBrand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == brand.Id);
            if (dbBrand == null) return NotFound();
            if (string.IsNullOrWhiteSpace(brand.Name))
            {
                ModelState.AddModelError("Name", "Bosluq Olmamalidir");
                return View(brand);
            }

            if (brand.Name.CheckString())
            {
                ModelState.AddModelError("Name", "Yalniz Herf Ola Biler");
                return View(brand);
            }

            if (await _context.Brands.AnyAsync(t => t.Id != brand.Id && t.Name.ToLower() == brand.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Alreade Exists");
                return View(brand);
            }
            dbBrand.Name = brand.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
