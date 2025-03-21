﻿using Cells;
using Skills.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Skills
{
    public class SkillsBook : MonoBehaviour
    {
        private PlayerSkillsHolder _playerSkillsHolder;
        public GameObject prefSkillCell;
       
        public SkillBranch branchFirst;
        public SkillBranch branchSecond;

        public Text weaponFirstTitle;
        public Text weaponSecondTitle;

        [SerializeField]
        private Transform _swordSkillsHolder;
        [SerializeField]
        private Transform _blantSkillsHolder;
        private SkillsRegistry _skillsRegistry;

        private List<SkillCell> _instatiatedCellSkills = new List<SkillCell>();

        private Canvas skillsBook;
        private UISort uISort;
        private DiContainer _container;

        [Inject]
        private void Construct(PlayerSkillsHolder playerSkillsHolder, DiContainer container, SkillsRegistry skillsRegistry)
        {
            _playerSkillsHolder = playerSkillsHolder;
            _skillsRegistry = skillsRegistry;

            _playerSkillsHolder.OnSkillsUpdate += OnSkillsUpdate;
            _container = container;
        }

       
        private void Awake() 
        {
            skillsBook = GetComponent<Canvas>();
            skillsBook.enabled = false;
            uISort = GetComponentInParent<UISort>();
        }

        private void Start()
        {
          

            ////Заполнить первую ветку умений
            //skillFirst = FillBranch(branchFirst, weaponFirstTitle, parentSkillFirst, skillFirst);

            ////Заполнить вторую ветку умений, если вторая ветка соответствует первой очистить
            //if (branchFirst != branchSecond)
            //{
            //    skillSecond = FillBranch(branchSecond, weaponSecondTitle, parentSkillSecond, skillSecond);
            //}
            //else
            //{
            //    ClearBranch(weaponSecondTitle, skillSecond);
            //}
        }

        private void OnSkillsUpdate()
        {
            foreach (SkillCell cell in _instatiatedCellSkills)
            {
                Destroy(cell.gameObject);
            }
            _instatiatedCellSkills.Clear();

            if(_playerSkillsHolder.Skills == null) return;

            foreach (int skillID in _playerSkillsHolder.Skills)
            {
                SkillData skill = _skillsRegistry.GetObjectByID(skillID);
                if (skill == null) continue;

                SkillCell skillCell = CreateSkillCell(skill);

                Transform parent = skill.Branch switch
                {
                    SkillBranch.Sword => _swordSkillsHolder,
                    SkillBranch.Blunt => _blantSkillsHolder,
                    _ => _swordSkillsHolder, //TODO: need default parent
                };

                skillCell.transform.SetParent(parent);

                _instatiatedCellSkills.Add(skillCell);
            }
        }

        private SkillCell CreateSkillCell(SkillData skill)
        {
            GameObject obj = _container.InstantiatePrefab(prefSkillCell);
            SkillCell skillCell = obj.GetComponent<SkillCell>();
            skillCell.PutSkill(skill);
            return skillCell;
        }


        //public static SkillCell GetSkillCellByID(int id)
        //{
        //    foreach(SkillCell cell in Instance.skillFirst)
        //    {
        //        if (cell.GetObjectID() == id)
        //            return cell;
        //    }
        //    return null;
        //}

        public void OpenCloseSkillsbook()
        {

            skillsBook.enabled = !skillsBook.enabled;

            if (skillsBook.enabled) uISort.PickUp(skillsBook);
        }

        //private void ClearBranch(Text text, List<SkillCell> list)
        //{
        //    if (list != null) DestroySkill(list);
        //    text.text = "";
        //}
        //private List<SkillCell> FillBranch(SkillBranch branch, Text text, Transform parent, List<SkillCell> list)
        //{
        //    if (list != null) DestroySkill(list);
        //    else list = new List<SkillCell>();

        //    switch (branch)
        //    {
        //        case SkillBranch.Sword:
        //            text.text = "Мечи";
        //            break;
        //        default:
        //            text.text = "";
        //            break;
        //    }

        //    foreach(Skill skill in _skillsList.skills)
        //    {
        //        if(skill.branch == branch)
        //        {
        //            if (prefSkillCell != null)
        //            {
        //                GameObject obj = Instantiate(prefSkillCell);
        //                obj.transform.SetParent(parent);
        //                SkillCell skillCell = obj.GetComponent<SkillCell>();
        //                skillCell.PutSkill(skill);
        //                list.Add(skillCell);
        //            }
        //            else Debug.LogError("error");
        //        }
        //    }
        //    return list;
        //}


        //private void DestroySkill(List<SkillCell> list)
        //{
        //    foreach (SkillCell obj in list) Destroy(obj.gameObject);
        //    list.Clear();
        //}
    }
}