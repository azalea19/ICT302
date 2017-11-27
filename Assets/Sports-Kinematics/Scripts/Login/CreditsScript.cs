using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The class allows to easily add new stakeholders to the 
/// project which will be displayed on the credits screen.
/// Author: Olesia Kochergina
/// Date: 16/09/2017
/// </summary>
public class CreditsScript : MonoBehaviour
{
    /// <summary>
    /// (roles, array of with a specific role)
    /// </summary>
    private Dictionary<string, string[]> m_credits;

    /// <summary>
    /// Reference to the roles game objects.
    /// </summary>
    private GameObject m_rolesGO;

    /// <summary>
    /// Reference to the contributors game object.
    /// </summary>
    private GameObject m_contrGO;

    /// <summary>
    /// Current position of the roles game object.
    /// </summary>
    private Vector3 m_rolesPos;

    /// <summary>
    /// Current position of the contributor game object.
    /// </summary>
    private Vector3 m_contrPos;

    /// <summary>
    /// initial y-axis position of the roles game object
    /// </summary>
    private float m_initRolesPosY;

    /// <summary>
    /// initial y-axis position of the contributor game object.
    /// </summary>
    private float m_initContrPosY;

    /// <summary>
    /// the last visible row
    /// </summary>
    private float m_offset;

    /// <summary>
    /// Initializes the instance's variables.
    /// </summary>
    void Start()
    {
        m_rolesGO = gameObject.transform.Find("Roles").gameObject;
        m_contrGO = gameObject.transform.Find("Contributors").gameObject;

        m_credits = new Dictionary<string, string[]>();
        AddRoles();
        float rows = DisplayCredits();
        m_rolesPos = m_rolesGO.transform.position;
        m_contrPos = m_contrGO.transform.position;

        m_initContrPosY = m_contrPos.y;
        m_initRolesPosY = m_rolesPos.y;
        //needs to be customized every time the dictionary is changed, defines the last visible row
        m_offset = Mathf.Abs(m_initRolesPosY) + rows * 0.1f + 5;
        OnEnable();

    }

    /// <summary>
    /// Sets the initial position of the credits text.
    /// </summary>
    void OnEnable()
    {
        m_rolesPos.y = m_initRolesPosY;
        m_contrPos.y = m_initContrPosY;
    }

    /// <summary>
    /// Allows for the credits page to be automatically scrolled vertically.
    /// </summary>
    void Update()
    {
        m_contrPos.y += 0.01f;
        m_rolesPos.y += 0.01f;
        m_rolesGO.transform.position = m_rolesPos;
        m_contrGO.transform.position = m_contrPos;
                
        if(m_rolesPos.y > m_offset)
        {
            m_rolesPos.y = m_initRolesPosY;
            m_contrPos.y = m_initContrPosY;
        }
    }

    /// <summary>
    /// Adds roles to different stakeholders of the project.
    /// </summary>
    private void AddRoles()
    {
        string[] client = { "Dr. Sean Muller" };
        string[] supervisor = { "Mr. Shri Rai" };
        string[] unitCoord = { "Peter Cole" };
        string[] sponsor = { "Murdoch University" };
        string[] team = { "Samuel Brownley", "Nathan Gane", "Olesia Kochergina" };
        string[] pTeam = {"Kinematics Research Solutions" };
        string[] testers = { "Oleg McNabb", "Mark Burns", "Jolon Theodore Martin", "Jordan Gumina-Wright", "Hugo Wai", "Aria Geramizadegan", "Joshua Jiow" };

        m_credits.Add("Client",client);
        m_credits.Add("Supervisor",supervisor);
        m_credits.Add("Unit Coordinator",unitCoord);
        m_credits.Add("Sponsor",sponsor);
        m_credits.Add("Development Team",team);
        m_credits.Add("Previous Developement Team",pTeam);
        m_credits.Add("Applecross Testers",testers);
        
    }

    
    /// <summary>
    /// Edits the text to be displayed to improve the visual appeal of it.
    /// </summary>
    /// <returns></returns>
    private int DisplayCredits()
    {
        int rows = 0;
        Text roles = m_rolesGO.GetComponent<Text>();
        Text contributors = m_contrGO.GetComponent<Text>();
        
        int paragraph = 0;
        foreach (KeyValuePair<string, string[]> entry in m_credits)
        {
            for (int i = 0; i < paragraph; i++)
            {
                roles.text += "\n";
            }
            roles.text += entry.Key.ToUpper() + "\n\n";
            paragraph = 0;
            contributors.text += "\n\n";
            for (int i = 0; i < entry.Value.Length; i++)
            {
                paragraph++;
                contributors.text += entry.Value[i].ToUpper() + "\n";
            }

            rows += paragraph+2;
        }
        for (int i = 0; i < paragraph; i++)
        {
            roles.text += "\n";
        }
        roles.text +=  "Semester 2, 2017";
        rows++;
        return rows;
    }
}
