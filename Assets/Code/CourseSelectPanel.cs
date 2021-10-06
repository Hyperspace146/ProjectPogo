using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourseSelectPanel : MonoBehaviour
{
    public GameObject Player;
    public Transform PogoCourseStart;
    public Transform GeneralCourseStart;

    public void TeleportToPogoCourse()
    {
        Player.transform.position = PogoCourseStart.position;
    }

    public void TeleportToGeneralCourse()
    {
        Player.transform.position = GeneralCourseStart.position;
    }
}
