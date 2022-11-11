using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefractionCube : ICube
{
    public Laser m_Laser;
    bool m_RefractionEnabled = false;
    private void Update()
    {
        m_Laser.gameObject.SetActive(m_RefractionEnabled);
        m_RefractionEnabled=false;
    }
    public void CreateRefraction()
    {
        if (m_RefractionEnabled) return;

        m_Laser.Shoot();

        m_RefractionEnabled=true;

    }
}
