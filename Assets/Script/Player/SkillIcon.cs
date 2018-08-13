using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    public static SkillIcon instance;

    [System.Serializable]
    public struct SkillContainer
    {
        public Image skillImg;
        public Image cdBar;
        public Text nowTime;
        public Text nowLevel;
    }

    public List<SkillContainer> skillContainer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SetSkillIcon(List<Sprite> _iconList)
    {
        for (int i = 0; i < skillContainer.Count; i++)
        {
            if (_iconList[i] != null)
                skillContainer[i].skillImg.sprite = _iconList[i];
        }
    }

    public void GoHintArea(GameObject _icon)
    {

    }

    public void GoHideArea(GameObject _icon)
    {

    }
}
