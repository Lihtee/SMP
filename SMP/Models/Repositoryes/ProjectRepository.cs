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

        public List<Project> Projects()
        {
            return cont.Project.OrderBy(p => p.IdProject).ToList();
        }

        public Project GetProjectById(int id)
        {
            return cont.Project.SingleOrDefault(p => p.IdProject == id);
        }

        public List<Project> GetProjectByParrentId(int parrentId)
        {
            return cont.Project.ToList().FindAll(p => p.parrentProject.IdProject == parrentId).ToList();
        }

        public List<Project> GetProjectByPersonId(int personId)
        {
            return (from team in cont.Team
                    where team.Person.IdPerson == personId
                    select team.Project).ToList();
            //return cont.Project.ToList().FindAll(p => p..IdProject == personId);
        }

        public void DeleteProject(int id)
        {
            cont.Project.Remove(cont.Project.Find(id));
            cont.SaveChanges();
        }

        public Project AddProject(string projectName, string description, 
            DateTime start, DateTime end, decimal plannnedBudget,
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
            };
            cont.Project.Add(p);
            cont.SaveChanges();
            return p;
        }

        public void DoneProject(int id)
        {
            GetProjectById(id).isDone = true;
            cont.SaveChanges();
        }

        public void UndoneProject(int id)
        {
            GetProjectById(id).isDone = false;
            cont.SaveChanges();
        }

        public void CloseProject(int id)
        {
            GetProjectById(id).isClose = true;
            cont.SaveChanges();
        }
    }
}