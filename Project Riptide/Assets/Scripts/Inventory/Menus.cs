using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menus : MonoBehaviour
{
    public void PlayerLeavePort()
    {
        GameObject[] allPorts = GameObject.FindGameObjectsWithTag("Port");
        foreach (GameObject port in allPorts)
        {
            if (port.GetComponent<PortManager>().inPort)
            {
                port.GetComponent<PortManager>().LeavePort();
                return;
            }
        }
    }

    public void EnablePortUI (GameObject windowToDisable)
    {
        windowToDisable.SetActive(false);
        transform.Find("PortMainMenu").gameObject.SetActive(true);
    }
}
