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

        public Project Parrent;
        public Project Work;
        public List<Project> Path;
        public List<WorkTableElement> WorkTableElements;
        public Dictionary<int, string> Executors;
        public int SelectedExecutorID;
        public string BackUrl;

        /// <summary>
        ///     Конструктор.
        /// </summary>
        /// <param name="id">ID работы.</param>
        /// <param name="backUrl">Куда перейдем, если нажмем "назад".</param>
        public WorkWievModel(int id, int parrentId, string backUrl = "")
        {
            // Todo обратная ссылка сейчас работает только для рута AddWork. Добавить такую штуку в рут Work и убрать рут AddWork.
            // Todo разобраться с параметрами конструкторов, чтобы не указывать лишние и не забыть нужные.
            Work = dataManager.projectRepository.GetProjectById(id);

            // Если не находим работу, то наверное создается новая.
            if (Work == null)
            {
                Parrent = dataManager.projectRepository.GetProjectById(parrentId);
            }

            BackUrl = backUrl;
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
            // Путь к работе.
            Path = new List<Project>();
            var workTemp = Work ?? Parrent;
            do
            {
                Path.Add(workTemp);
                workTemp = workTemp.parrentProject;
            } while (workTemp != null);

            Path.Reverse();

            // Список исполнителей.
            Executors = new Dictionary<int, string>();
            if (!dataManager.projectRepository.GetProjectsByParrentId(Work?.IdProject ?? -1).Any())
            {
                var innerTeams = dataManager.teamRepository.GetTeamsOfMainProject(Work?.IdProject ?? Parrent.IdProject);
                foreach (var t in innerTeams)
                {
                    Executors.Add(t.Person.IdPerson, $"{t.Person.firstName} {t.Person.surName}");
                }
            }

            // Все остальное - для уже существующей работы.
            // Todo - можно нормально разбить на методы и конструкторы, или понаследовать ViewModel.
            if (Work == null)
            {
                return;
            }

            // Подработы.
            var subworks = dataManager.projectRepository.GetProjectsByParrentId(Work.IdProject)
                .OrderBy(w => w.endDateTime);

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

            // Выбранный исполнитель.
            var team = dataManager.teamRepository.GetTeamsByProject(Work.IdProject).ToList();
            SelectedExecutorID = 1;
            if (team.Count == 1)
            {
                SelectedExecutorID = team.First().Person.IdPerson;
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