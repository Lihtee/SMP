using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using SMP.Models;

namespace SMP.ViewModels
{
    /// <summary>
    ///     Модель работы.
    /// </summary>
    public class WorkWievModel
    {
        private static readonly DataManager dataManager = new DataManager();

        public Project Work;
        public List<Project> Path;
        public List<WorkTableElement> WorkTableElements;
        public Dictionary<int, string> Executors;
        public int SelectedExecutorID;

        /// <summary>
        ///     Конструктор.
        /// </summary>
        /// <param name="id">ID работы.</param>
        public WorkWievModel(int id)
        {
            Work = dataManager.projectRepository.GetProjectById(id);
            BuildModel();
        }

        /// <summary>
        ///     Конструктор.
        /// </summary>
        /// <param name="work">Работа.</param>
        public WorkWievModel(Project work)
        {
            Work = work;
            BuildModel();
        }


        /// <summary>
        ///     Строит модель.
        /// </summary>
        private void BuildModel()
        {
            // Команды главного проекта.
            var innerTeams = dataManager.teamRepository.GetTeamsOfMainProject(Work.IdProject)
                .OrderBy(w => w.Project.endDateTime)
                .ToList();

            // Подработы.
            var subworks = dataManager.projectRepository.GetProjectsByParrentId(Work.IdProject)
                .OrderBy(w => w.endDateTime);

            // Путь к работе.
            Path = new List<Project>();
            var workTemp = Work;
            do
            {
                Path.Add(workTemp);
                workTemp = workTemp.parrentProject;
            } while (workTemp != null);

            Path.Reverse();

            // Таблица подработ.
            WorkTableElements = new List<WorkTableElement>();
            foreach (var subwork in subworks)
            {
                var workTableElement = new WorkTableElement
                {
                    Work = subwork
                };

                // Записываем исполнителя в таблицу подработ.
                var subworkTeams = dataManager.teamRepository.GetTeams()
                    .Where(t => t.Project.IdProject == subwork.IdProject)
                    .ToList();
                if (subworkTeams.Count == 1)
                {
                    var exec = subworkTeams.First().Person;
                    workTableElement.ExecutorName = $"{exec.firstName} {exec.surName}";
                }
                else
                {
                    workTableElement.ExecutorName = "";
                }

                WorkTableElements.Add(workTableElement);
            }

            // Список исполнителей.
            Executors = new Dictionary<int, string>();
            if (!dataManager.projectRepository.GetProjectsByParrentId(Work.IdProject).Any())
            {
                foreach (var t in innerTeams)
                {
                    Executors.Add(t.Person.IdPerson, $"{t.Person.firstName} {t.Person.surName}");
                } 
            }

            // Команда текущей работы.
            var team = dataManager.teamRepository.GetTeamByWork(Work.IdProject);
            SelectedExecutorID = 1;
            if (team != null)
            {
                SelectedExecutorID = team.Person.IdPerson;
            }

           
        }

        /// <summary>
        /// Элемент таблицы подработ.
        /// </summary>
        public class WorkTableElement
        {
            public Project Work;

            public String ExecutorName;
        }
    }
}