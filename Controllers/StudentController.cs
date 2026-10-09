using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyWebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyWebApp.Controllers
{
    public class StudentController : Controller
    {
        private static List<Student> listStudents = new List<Student>();

        public StudentController()
        {
            // Khởi tạo danh sách 4 sinh viên mẫu nếu danh sách đang trống
            if (listStudents.Count == 0)
            {
                listStudents = new List<Student>()
                {
                    new Student()
                    {
                        Id = 101,
                        Name = "Hải Nam",
                        Branch = Branch.IT,
                        Gender = Gender.Male,
                        IsRegular = true,
                        Address = "A1-2018",
                        Email = "nam@g.com",
                        DateOfBorth = new DateTime(2001, 1, 1, 12, 0, 0)
                    },
                    new Student()
                    {
                        Id = 102,
                        Name = "Minh Tú",
                        Branch = Branch.BE,
                        Gender = Gender.Female,
                        IsRegular = true,
                        Address = "A1-2019",
                        Email = "tu@g.com",
                        DateOfBorth = new DateTime(2001, 1, 1, 12, 0, 0)
                    },
                    new Student()
                    {
                        Id = 103,
                        Name = "Hoàng Phong",
                        Branch = Branch.CE,
                        Gender = Gender.Male,
                        IsRegular = false,
                        Address = "A1-2020",
                        Email = "phong@g.com",
                        DateOfBorth = new DateTime(2001, 1, 1, 12, 0, 0)
                    },
                    new Student()
                    {
                        Id = 104,
                        Name = "Xuân Mai",
                        Branch = Branch.EE,
                        Gender = Gender.Female,
                        IsRegular = false,
                        Address = "A1-2021",
                        Email = "mai@g.com",
                        DateOfBorth = new DateTime(2001, 1, 1, 12, 0, 0)
                    }
                };
            }
        }

        // Cấu hình Route cho Student/Index thành Admin/Student/List
        [HttpGet]
        [Route("Student/Index")]
        //[Route("Admin/Student/List", Name = "StudentList")]
        public IActionResult Index()
        {
            return View(listStudents);
        }

        [HttpGet]
        [Route("Student/Create")]
        //[Route("Admin/Student/Add", Name = "StudentAdd")]
        public IActionResult Create()
        {
            PopulateDropdownData();
            return View();
        }

        [HttpPost]
        [Route("Student/Create")]
        //[Route("Admin/Student/Add")]
        public IActionResult Create(Student s, IFormFile? avatarFile)
        {
            if (ModelState.IsValid)
            {
                s.Id = listStudents.Last<Student>().Id + 1;
                // Xử lý upload ảnh đại diện (Mở rộng Mục 9)
                if (avatarFile != null && avatarFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(avatarFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        avatarFile.CopyTo(fileStream);
                    }
                    s.Avatar = "/images/" + uniqueFileName;
                }

                listStudents.Add(s);
                return RedirectToAction("Index");
            }

            PopulateDropdownData();
            return View();
        }

        private void PopulateDropdownData()
        {
            ViewBag.AllGenders = Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList();
            ViewBag.AllBranches = new List<SelectListItem>
        {
            new SelectListItem { Text = "IT", Value = "1" },
            new SelectListItem { Text = "BE", Value = "2" },
            new SelectListItem { Text = "CE", Value = "3" },
            new SelectListItem { Text = "EE", Value = "4" }
        };
            }
        }
}

