using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMP.Models.Repositoryes
{
    public class ProjectRepository
    {
        static ModelContainer cont = new ModelContainer();

        public ProjectRepository(ModelContainer _cont)
        {
            cont = _cont;
        }

        /// <summary>
        /// Возвращает список всех проектов
        /// </summary>
        /// <returns>Список всех проектов</returns>
        public List<Project> GetProjects()
        {
            return cont.Project.OrderBy(p => p.IdProject).ToList();
        }

        /// <summary>
        /// Возвращает проект во его Id
        /// </summary>
        /// <param name="id">Id проекта</param>
        /// <returns>Проект</returns>
        public Project GetProjectById(int id)
        {
            return cont.Project.SingleOrDefault(p => p.IdProject == id);
        }

        /// <summary>
        /// Возвращает список работ по Id родительского проекта 
        /// </summary>
        /// <param name="parrentId">Id родительского проекта</param>
        /// <returns>Список проектов</returns>
        public List<Project> GetProjectsByParrentId(int parrentId)
        {
            return cont.Project.Where(p => p.parrentProject.IdProject == parrentId).ToList();
        }

        /// <summary>
        /// Возвращает список проектов/работ по Id исполнителя 
        /// </summary>
        /// <param name="personId">Id исполнителя</param>
        /// <returns>Список проектов</returns>
        public List<Project> GetProjectsByPersonId(int personId)
        {
            return (from team in cont.Team
                    where team.Person.IdPerson == personId
                    select team.Project).ToList();
            //return cont.Project.ToList().FindAll(p => p..IdProject == personId);
        }

        /// <summary>
        /// Удаляет проект/работу по Id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteProject(int id, int startID)
        {
            foreach (var p in GetProjectsByParrentId(id))
            {
                DeleteProject(p.IdProject, startID);
            }

            foreach (var t in cont.Team.Where(team => team.Project.IdProject == id).ToList())
            {
                cont.Team.Remove(t);
            }

            foreach (var a in cont.Addiction.Where(addiction => addiction.lastProject.IdProject == id || addiction.nextProject.IdProject == id).ToList())
            {
                cont.Addiction.Remove(a);
            }

            cont.Project.Remove(cont.Project.Find(id));

            if (id == startID)
                cont.SaveChanges();
        }

        /// <summary>
        /// Добавляет проект в базу
        /// </summary>
        /// <param name="projectName">Название проекта</param>
        /// <param name="description">Описание проекта (может отсутствовать)</param>
        /// <param name="start">Время начала проекта</param>
        /// <param name="end">Время окончания проекта</param>
        /// <param name="plannnedBudget">Плановый бюджет</param>
        /// <param name="parrentId">Id родительского проекта</param>
        /// <returns>Возвращает добавленный проект</returns>
        public Project AddProject(string projectName, string description,
            DateTime start, DateTime end, decimal plannnedBudget, int reserve,
            int parrentId)
        {

            Project p = new Project
            {
                projectName = projectName,
                description = description,
                startDateTime = start,
                endDateTime = end,

                plannedBudget = plannnedBudget,
                realBudget = null,
                parrentProject = GetProjectById(parrentId),
                reserve = reserve,
            };
            cont.Project.Add(p);
            cont.SaveChanges();
            return p;
        }

        /// <summary>
        /// Добавляет проект в базу
        /// </summary>
        /// <param name="projectName">Название проекта</param>
        /// <param name="description">Описание проекта (может отсутствовать)</param>
        /// <param name="start">Время начала проекта</param>
        /// <param name="end">Время окончания проекта</param>
        /// <param name="plannnedBudget">Плановый бюджет</param>
        /// <returns>Возвращает добавленный проект</returns>
        public Project AddProject(string projectName, string description,
            DateTime start, DateTime end, decimal plannnedBudget, int reserve)
        {

            Project p = new Project
            {
                projectName = projectName,
                description = description,
                startDateTime = start,
                endDateTime = end,

                plannedBudget = plannnedBudget,
                realBudget = null,
                parrentProject = null,
                reserve = reserve,
            };
            cont.Project.Add(p);
            cont.SaveChanges();
            return p;
        }

        /// <summary>
        /// Изменяет проект из базы
        /// </summary>
        /// <param name="id">Id пороекта</param>
        /// <param name="projectName">Название проекта</param>
        /// <param name="description">Описание проекта (может отсутствовать)</param>
        /// <param name="start">Время начала проекта</param>
        /// <param name="end">Время окончания проекта</param>
        /// <param name="plannnedBudget">Плановый бюджет</param>
        /// <param name="parrentId">Id родительского проекта</param>
        /// <returns>Возвращает изменённый проект</returns>
        public Project EditProject(int id, string projectName, string description,
            DateTime start, DateTime end, decimal plannnedBudget, int reserve)
        {
            Project p = GetProjectById(id);

            p.projectName = projectName;
            p.description = description;
            p.startDateTime = start;
            p.endDateTime = end;

            p.plannedBudget = plannnedBudget;
            p.realBudget = null;
            p.reserve = reserve;
            
            cont.SaveChanges();
            return p;
        }

        /// <summary>
        /// Отмечает проект/работу, как не выполненый (Только для исполнотеля)
        /// </summary>
        /// <param name="id">Id проекта/работы</param>
        public void DoneProject(int id)
        {
            GetProjectById(id).isDone = true;
            cont.SaveChanges();
        }

        /// <summary>
        /// Отмечает проект/работу, как не выполненый (Только для менеджера)
        /// </summary>
        /// <param name="id">Id проекта/работы</param>
        public void UndoneProject(int id)
        {
            GetProjectById(id).isDone = false;
            cont.SaveChanges();
        }

        /// <summary>
        /// Закрывает проект (Только для менеджера)
        /// </summary>
        /// <param name="id">Id проекта/работы</param>
        public void CloseProject(int id)
        {
            GetProjectById(id).isClose = true;
            cont.SaveChanges();
        }

        /// <summary>
        /// Возвращает проект верхнего уровня для проекта нижнего уровня.
        /// </summary>
        /// <param name="child">Проект нижнего уровня</param>
        /// <returns></returns>
        public Project GetInnerProject(Project child)
        {
            if (child == null)
                return null;
            Project res = child;
            while (res.parrentProject != null)
            {
                res = res.parrentProject;
            }
            return res;
        }

        /// <summary>
        /// Возвращает проект верхнего уровня для проекта нижнего уровня.
        /// </summary>
        /// <param name="child">Id проекта нижнего уровня</param>
        /// <returns></returns>
        public Project GetInnerProject(int childId)
        {
            return GetInnerProject(GetProjectById(childId));
        }
    }
}