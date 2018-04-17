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
            return cont.Team.OrderBy(p => p.IdTeam).ToList();
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
            return t;
        }
    }
}