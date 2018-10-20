using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMP.Models.Repositoryes
{
    public class TeamRepository
    {
        static ModelContainer cont = new ModelContainer();

        public TeamRepository(ModelContainer _cont)
        {
            cont = _cont;
        }

        /// <summary>
        /// Возвращает все команды из базы
        /// </summary>
        /// <returns>Список команд</returns>
        public List<Team> GetTeams()
        {
            return cont.Team.OrderBy(t => t.IdTeam).ToList();
        }

        /// <summary>
        /// Возвращает команду по Id
        /// </summary>
        /// <param name="teamId">Id команды</param>
        /// <returns></returns>
        public Team GetTeamById(int teamId)
        {
            return cont.Team.ToList().Find(t => t.IdTeam == teamId);
        }

        /// <summary>
        /// Возвращает команду по Id исполнителя и проекта
        /// </summary>
        /// <param name="personId">Id исполнителя</param>
        /// <param name="projectId">Id проекта</param>
        /// <returns>Команду</returns>
        public Team GetTeam(int personId, int projectId)
        {
            return cont.Team.ToList().Find(t => t.Person.IdPerson == personId && t.Project.IdProject == projectId);
        }

        /// <summary>
        /// Возвращает команды исполнителя
        /// </summary>
        /// <param name="personId">Id исполнителя</param>
        /// <returns>Список команд</returns>
        public List<Team> GetTeamsByPerson(int personId)
        {
            return cont.Team.ToList().FindAll(t => t.Person.IdPerson == personId);
        }

        /// <summary>
        /// Возвращает команду проекта/работы
        /// </summary>
        /// <param name="projectId">Id проекта</param>
        /// <returns>Список команд</returns>
        public List<Team> GetTeamsByProject(int projectId)
        {
            return cont.Team.ToList().FindAll(t => t.Project.IdProject == projectId);
        }

        /// <summary>
        /// Возвращает команду проекта/работы
        /// </summary>
        /// <param name="projectId">Id проекта</param>
        /// <returns>Список команд</returns>
        public List<Team> GetTeamsByParrentProject(int projectId)
        {
            return (from t in cont.Team
                    where t.Project.parrentProject.IdProject == projectId
                    select t).ToList();
        }

        /// <summary>
        /// Возвращает команд главного проекта
        /// </summary>
        /// <param name="projectId">Id проекта/работы</param>
        /// <returns>Список команд</returns>
        public List<Team> GetTeamsByParrentProject(int projectId)
        {
            Project project = cont.Project.SingleOrDefault(p => p.IdProject == id);
            while (project.parrentProject == null)
            {
                project = project.parrentProject;
            }
            return GetTeamsByProject(project.IdProject);
        }

        /// <summary>
        /// Команду работы
        /// </summary>
        /// <param name="projectId">Id проекта</param>
        /// <returns>Команда</returns>
        public Team GetTeamByWork(int projectId)
        {
            return cont.Team.ToList().SingleOrDefault(t => t.Project.IdProject == projectId);
        }

        /// <summary>
        /// Добавляет команду в базу
        /// </summary>
        /// <param name="personId">Id исполнителя</param>
        /// <param name="projectId">Id проекта</param>
        /// <returns>Добавленную команду</returns>
        public Team AddTeam(int personId, int projectId)
        {
            Team t = new Team
            {
                Person = cont.Person.FirstOrDefault(p => p.IdPerson == personId),
                Project = cont.Project.FirstOrDefault(p => p.IdProject == projectId),
            };
            cont.Team.Add(t);
            cont.SaveChanges();
            return t;
        }

        /// <summary>
        /// Удаляет команду из базы
        /// </summary>
        /// <param name="teamId">Id команды</param>
        public void DeleteTeam(int teamId)
        {
            cont.Team.Remove(GetTeamById(teamId));
            cont.SaveChanges();
        }
    }
}