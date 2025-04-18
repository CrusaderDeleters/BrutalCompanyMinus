﻿using BepInEx.Configuration;
using HarmonyLib;
using System;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.MonoBehaviours
{
    [HarmonyPatch]
    internal class ArtilleryShell : MonoBehaviour
    {
        public Transform transform;

        public static ConfigEntry<float> speed;

        private float timeTillExplode = 1.0f;

        public Vector3 target = Vector3.zero;

        private void Start()
        {
            transform.LookAt(target);
            timeTillExplode = Vector3.Distance(transform.position, target) / speed.Value;
        }

        private void Update()
        {
            transform.position += transform.forward * Time.deltaTime * speed.Value;
            if (timeTillExplode > 0)
            {
                timeTillExplode -= Time.deltaTime;
            }
            else
            {
                try
                {
                    Landmine.SpawnExplosion(transform.position, true, 5, 6);
                }
                catch
                {

                }
                Destroy(transform.gameObject);
            }

        }

        public static void FireAt(Vector3 at, Vector3 from) => Net.Instance.FireAtServerRpc(at, from);
    }
}
