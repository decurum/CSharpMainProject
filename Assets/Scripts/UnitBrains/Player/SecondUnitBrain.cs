﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Codice.Client.BaseCommands.CheckIn.CodeReview;
using Model;
using Model.Runtime.Projectiles;
using UnityEngine;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;
        
        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            ///////////////////////////////////////
            ///// Homework 1.3
            ///////////////////////////////////////           
            var projectile = CreateProjectile(forTarget);
            AddProjectileToList(projectile, intoList);

            float temp = GetTemperature();

            if (temp >= overheatTemperature) return;

            IncreaseTemperature();

            for (int i = 0; i <= temp; i++)

            {

                projectile = CreateProjectile(forTarget);

                AddProjectileToList(projectile, intoList);

            }
            ///////////////////////////////////////
        }

        public override Vector2Int GetNextStep()
        {
            return base.GetNextStep();
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            ////////List<Vector2Int> result = GetReachableTargets();
            ////////float aim = float.MaxValue;
            ////////Vector2Int closer = Vector2Int.zero;
            //////////foreach (var certainAim in result) { float z = DistanceToOwnBase(certainAim); if (z < aim) {aim = z; closer = certainAim;}}
            //////////if (aim != float.MaxValue) {  result.Clear(); result.Add(closer);}
            //////////while (result.Count > 1) //{ //    result.RemoveAt(result.Count - 1); //}
            ////////return result;

            List<Vector2Int> searchAims = new();
            List<Vector2Int> unfoundAims = new();
            var allAims = GetAllTargets();
            var nearest = float.MaxValue;
            var rtM = runtimeModel.RoMap.Bases;
            
            if (!allAims.Any())
            {
                searchAims.Add(rtM [IsPlayerUnitBrain ? Model.RuntimeModel.BotPlayerId : Model.RuntimeModel.PlayerId]);
                return searchAims;
            }
            Vector2Int nearestAim = new Vector2Int(int.MinValue, int.MinValue);
            foreach (var target in allAims)
            {
                var aimPath = DistanceToOwnBase(target);
                if (aimPath < nearest)
                {
                    nearest = aimPath;
                    nearestAim = target;
                }
            }
            if (nearest != float.MaxValue)
            {
                unfoundAims.Add(nearestAim);
                if (IsTargetInRange(nearestAim))
                {
                    searchAims.Add(nearestAim);
                }
            }

            return searchAims;
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {              
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown/10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature()
        {
            if(_overheated) return (int) OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}