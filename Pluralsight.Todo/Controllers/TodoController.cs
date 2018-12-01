using Pluralsight.Todo.Models;
using Pluralsight.Todo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pluralsight.Todo.Controllers
{
    public class TodoController : Controller
    {
        // GET: Todo
        public ActionResult Index()
        {
            var repository = new TodoRepository();
            var entities = repository.All();
            var models = entities.Select(x => new TodoModel {
                Id = x.RowKey,
                Group = x.PartitionKey,
                Content = x.Content,
                Due = x.Due,
                Completed = x.Completed,
                Timestamp = x.Timestamp
            });
            return View(models);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(TodoModel model)
        {
            var repository = new TodoRepository();

            repository.CreateOrUpdate(new TodoEntity {
                PartitionKey = model.Group,
                RowKey = Guid.NewGuid().ToString(),
                Content = model.Content,
                Due = model.Due
            });

            return RedirectToAction("Index");
        }

        public ActionResult ConfirmDelete(string id, string group)
        {
            var repository = new TodoRepository();
            var item = repository.Get(group, id);

            return View("Delete", new TodoModel
            {
                Id = item.RowKey,
                Group = item.PartitionKey,
                Content = item.Content,
                Due = item.Due,
                Completed = item.Completed,
                Timestamp = item.Timestamp
            });
        }

        [HttpPost]
        public ActionResult Delete(string id, string group)
        {
            var repository = new TodoRepository();
            var item = repository.Get(group, id);
            repository.Delete(item);

            return RedirectToAction("Index");
        }

        public ActionResult Edit(string id, string group)
        {
            var repository = new TodoRepository();
            var item = repository.Get(group, id);

            return View(new TodoModel
            {
                Id = item.RowKey,
                Group = item.PartitionKey,
                Content = item.Content,
                Due = item.Due,
                Completed = item.Completed,
                Timestamp = item.Timestamp
            });
        }

        [HttpPost]
        public ActionResult Edit(TodoModel model)
        {
            var repository = new TodoRepository();
            var item = repository.Get(model.Group, model.Id);
            item.Completed = model.Completed;
            item.Content = model.Content;
            item.Due = model.Due;

            repository.CreateOrUpdate(item);

            return RedirectToAction("Index");
        }
    }
}