using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Fasterflect;

namespace SampleMapEditor
{
    public class ByamlSerialize
    {
        // Special Deserialize for Stage Layouts
        public static void SpecialDeserialize(object section, dynamic value)
        {
            if (value is IList)
            {
                var props = section.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                DbgSetValues(props[0], props[0].PropertyType, section, value);
                return;
            }

            Dictionary<string, dynamic> bymlProperties;

            if (value is Dictionary<string, dynamic>) bymlProperties = (Dictionary<string, dynamic>)value;
            else throw new Exception("Not a dictionary");

            //Console.ResetColor();
            Console.WriteLine();
            /*if (section is MuElement)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"section is type: {section}");
                string classname = MuElement.GetActorClassName((MuElement)section);
                Console.WriteLine(classname);
                Console.ResetColor();
                switch (classname)
                {
                    case "Field":
                        break;
                    case "DesignerObj":
                        Console.WriteLine($"RailableParams::AttCalc: {((DesignerObj)section).RailableParams__AttCalc}");
                        break;
                    default:
                        break;
                }
            }*/


            if (section is IByamlSerializable)
                ((IByamlSerializable)section).DeserializeByaml(bymlProperties);

            var properties = section.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var fields = section.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            for (int i = 0; i < fields.Length; i++)
            {
                //Only load properties with byaml attributes
                var byamlAttribute = fields[i].GetCustomAttribute<ByamlMember>();
                if (byamlAttribute == null)
                    continue;

                Type type = fields[i].FieldType;
                Type nullableType = Nullable.GetUnderlyingType(type);
                if (nullableType != null)
                    type = nullableType;

                //Set custom keys as property name if used
                string name = byamlAttribute.Key != null ? byamlAttribute.Key : fields[i].Name;

                //Skip properties that are not present
                if (!bymlProperties.ContainsKey(name))
                    continue;

                DbgSetValues(fields[i], type, section, bymlProperties[name]);
            }

            for (int i = 0; i < properties.Length; i++)
            {
                //Get a property that stores the current dynamic dictionary
                var byamlPropertiesAttribute = properties[i].GetCustomAttribute<ByamlPropertyList>();
                if (byamlPropertiesAttribute != null)
                {
                    //Store the whole dynamic into the dictionary of the property
                    //All properties will be stored then saved back if none are serialized elsewhere in the class
                    properties[i].SetValue(section, value);
                    continue;
                }

                //Only load properties with byaml attributes
                var byamlAttribute = properties[i].GetCustomAttribute<ByamlMember>();
                if (byamlAttribute == null)
                    continue;

                Type type = properties[i].PropertyType;
                Type nullableType = Nullable.GetUnderlyingType(type);
                if (nullableType != null)
                    type = nullableType;

                //Console.WriteLine($"properties[{i}] type: {type}");

                //Set custom keys as property name if used
                string name = byamlAttribute.Key != null ? byamlAttribute.Key : properties[i].Name;

                //Skip properties that are not present
                if (!bymlProperties.ContainsKey(name))
                    continue;

                //Make sure the property has a setter and getter
                if (!properties[i].CanRead || !properties[i].CanWrite)
                {
                    throw new Exception(
                        $"Property {value}.{properties[i].Name} requires both a getter and setter to be used for dserialization.");
                }

                DbgSetValues(properties[i], type, section, bymlProperties[name]);
            }
        }



        static void DbgSetValues(object property, Type type, object section, dynamic value)
        {
            //Console.WriteLine($"DbgSetValues ~ {property} % {type} % {section} % {value}");

            //if (section == typeof(MuElement))
            /*if (section is MuElement)
            {
                Console.WriteLine($"*** CURRENT TYPE IS MuElement ***");
            }*/

            // If the value to set is a pathpoint
            if (value is IList<ByamlExt.Byaml.ByamlPathPoint>)
            {
                DbgSetValue(property, section, value);
            }
            else if (value is IList<dynamic>)
            {
                // In Splatoon 3, the Translate_Rotate_Scale data are in lists
                if (type == typeof(ByamlVector3F))
                {
                    var vec3 = new ByamlVector3F(value[0], value[1], value[2]);
                    DbgSetValue(property, section, vec3);
                    return;
                }

                // The following code is for actors
                var list = (IList<dynamic>)value;
                var array = InstantiateType<IList>(type);

                Type elementType = type.GetTypeInfo().GetElementType();
                if (type.IsGenericType && elementType == null)
                    elementType = type.GetGenericArguments()[0];

                for (int j = 0; j < list.Count; j++)
                {
                    // Set Type (for Actors)
                    if (property.GetPropertyValue("Name").ToString() == "Actors")
                    {
                        string className = MuElement.GetActorClassName(list[j]);
                        if (className.Length == 0) className = list[j]["Name"];

                        SetMapObjType(ref elementType, list[j]["Name"]);
                    }

                    // (?)
                    if (list[j] is IDictionary<string, dynamic>)
                    {
                        var instance = CreateInstance(elementType);
                        SpecialDeserialize(instance, list[j]);
                        array.Add(instance);
                    }
                    else if (list[j] is IList<dynamic>)
                    {
                        var subList = list[j] as IList<dynamic>;

                        var instance = CreateInstance(elementType);
                        if (instance is IList)
                        {
                            for (int k = 0; k < subList.Count; k++)
                                ((IList)instance).Add(subList[k]);
                        }
                        array.Add(instance);
                    }
                    else
                        array.Add(list[j]);
                }

                DbgSetValue(property, section, array);
            }
            else if (value is IDictionary<string, dynamic>)
            {
                if (type == typeof(MuObj.TeamNode))
                {
                    var values = (IDictionary<string, dynamic>)value;
                    var vec3 = new MuObj.TeamNode() { Team = values["Team"] };
                    DbgSetValue(property, section, vec3);
                }
                else if (type == typeof(string))
                {

                }
                else
                {
                    var instance = CreateInstance(type);
                    SpecialDeserialize(instance, value);
                    DbgSetValue(property, section, instance);
                }
            }
            else
                DbgSetValue(property, section, value);
        }

        static void DbgSetValue(object property, object instance, object value)
        {
            //Console.WriteLine($"DbgSetValue ~ {property} % {instance} % {value}");
            if (property is PropertyInfo)
            {
                Type nullableType = Nullable.GetUnderlyingType(((PropertyInfo)property).PropertyType);
                if (nullableType != null && nullableType.GetTypeInfo().IsEnum)
                {
                    value = Enum.ToObject(nullableType, value);
                }
            }

            if (property is PropertyInfo)
                ((PropertyInfo)property).SetValue(instance, value);
            else if (property is FieldInfo)
                ((FieldInfo)property).SetValue(instance, value);
        }



        public static void SetMapObjType(ref Type type, string className)
        {
            Console.WriteLine($"Using Class Name: {className}");
            switch (className)
            {
                case "ActionAreaSwitchForPlayer":
                    type = typeof(ActionAreaSwitchForPlayer);
                    break;

                case "ActorPaintChecker":
                    type = typeof(ActorPaintChecker);
                    break;

                case "AerialRing":
                    type = typeof(AerialRing);
                    break;

                case "AirBallParent":
                    type = typeof(AirBallParent);
                    break;

                case "AirBall_IgnoreCulling":
                    type = typeof(AirBall_IgnoreCulling);
                    break;

                case "AmbientSoundArea":
                    type = typeof(AmbientSoundArea);
                    break;

                case "AttackPermitNumChangeArea":
                    type = typeof(AttackPermitNumChangeArea);
                    break;

                case "BallReturn12x4":
                    type = typeof(BallReturn12x4);
                    break;

                case "BallReturn4x4":
                    type = typeof(BallReturn4x4);
                    break;

                case "BallReturn8x4":
                    type = typeof(BallReturn8x4);
                    break;

                case "BGActorDObj_SdodrBigFishCoelacanth_SwimRailA":
                    type = typeof(BGActorDObj_SdodrBigFishCoelacanth_SwimRailA);
                    break;

                case "BGActorDObj_SdodrBigFishCoelacanth_SwimRailB":
                    type = typeof(BGActorDObj_SdodrBigFishCoelacanth_SwimRailB);
                    break;

                case "BGActorDObj_SdodrBigFishShark":
                    type = typeof(BGActorDObj_SdodrBigFishShark);
                    break;

                case "BGActorDObj_SdodrBigFishShark_SwimRailA":
                    type = typeof(BGActorDObj_SdodrBigFishShark_SwimRailA);
                    break;

                case "BGActorDObj_SdodrBigFishShark_SwimRailB":
                    type = typeof(BGActorDObj_SdodrBigFishShark_SwimRailB);
                    break;

                case "BGActorDObj_SdodrBigFishShark_SwimRailC":
                    type = typeof(BGActorDObj_SdodrBigFishShark_SwimRailC);
                    break;

                case "BGActorDObj_SdodrEffectLocatorC_SwimRailA":
                    type = typeof(BGActorDObj_SdodrEffectLocatorC_SwimRailA);
                    break;

                case "BGActorDObj_SdodrEffectLocatorC_SwimRailB":
                    type = typeof(BGActorDObj_SdodrEffectLocatorC_SwimRailB);
                    break;

                case "BGActorDObj_SdodrEffectLocatorC_SwimRailC":
                    type = typeof(BGActorDObj_SdodrEffectLocatorC_SwimRailC);
                    break;

                case "BGActorDObj_SdodrNapoleon_SwimRailA":
                    type = typeof(BGActorDObj_SdodrNapoleon_SwimRailA);
                    break;

                case "BGActorDObj_SdodrNapoleon_SwimRailB":
                    type = typeof(BGActorDObj_SdodrNapoleon_SwimRailB);
                    break;

                case "BGActorDObj_SdodrNapoleon_SwimRailC":
                    type = typeof(BGActorDObj_SdodrNapoleon_SwimRailC);
                    break;

                case "BGActorDObj_SdodrPirarucu_SwimRailA":
                    type = typeof(BGActorDObj_SdodrPirarucu_SwimRailA);
                    break;

                case "BGActorDObj_SdodrPirarucu_SwimRailB":
                    type = typeof(BGActorDObj_SdodrPirarucu_SwimRailB);
                    break;

                case "BGActorDObj_SdodrPirarucu_SwimRailC":
                    type = typeof(BGActorDObj_SdodrPirarucu_SwimRailC);
                    break;

                case "BGActorDObj_SdodrSardine_SwimRailA":
                    type = typeof(BGActorDObj_SdodrSardine_SwimRailA);
                    break;

                case "BGActorDObj_SdodrSardine_SwimRailB":
                    type = typeof(BGActorDObj_SdodrSardine_SwimRailB);
                    break;

                case "BGActorDObj_SdodrSardine_SwimRailC":
                    type = typeof(BGActorDObj_SdodrSardine_SwimRailC);
                    break;

                case "BGActorDObj_SdodrSunfish_SwimRailA":
                    type = typeof(BGActorDObj_SdodrSunfish_SwimRailA);
                    break;

                case "BGActorDObj_SdodrSunfish_SwimRailB":
                    type = typeof(BGActorDObj_SdodrSunfish_SwimRailB);
                    break;

                case "BGActorDObj_SdodrSunfish_SwimRailC":
                    type = typeof(BGActorDObj_SdodrSunfish_SwimRailC);
                    break;

                case "BGActorDObj_SdodrTuna_SwimRailA":
                    type = typeof(BGActorDObj_SdodrTuna_SwimRailA);
                    break;

                case "BGActorDObj_SdodrTuna_SwimRailB":
                    type = typeof(BGActorDObj_SdodrTuna_SwimRailB);
                    break;

                case "BGActorDObj_SdodrTuna_SwimRailC":
                    type = typeof(BGActorDObj_SdodrTuna_SwimRailC);
                    break;

                case "BGActorDObj_SdodrWhaleshark_SwimRailA":
                    type = typeof(BGActorDObj_SdodrWhaleshark_SwimRailA);
                    break;

                case "BGActorDObj_SdodrWhaleshark_SwimRailB":
                    type = typeof(BGActorDObj_SdodrWhaleshark_SwimRailB);
                    break;

                case "BGActorDObj_SdodrWhaleshark_SwimRailC":
                    type = typeof(BGActorDObj_SdodrWhaleshark_SwimRailC);
                    break;

                case "BgmArea":
                    type = typeof(BgmArea);
                    break;

                case "BlackLight":
                    type = typeof(BlackLight);
                    break;

                case "Blowouts":
                    type = typeof(Blowouts);
                    break;

                case "BlowoutsWide":
                    type = typeof(BlowoutsWide);
                    break;

                case "BombFlowerParent":
                    type = typeof(BombFlowerParent);
                    break;

                case "BombFlowerSdodr":
                    type = typeof(BombFlowerSdodr);
                    break;

                case "BoxSet":
                    type = typeof(BoxSet);
                    break;

                case "CanBuildMachine":
                    type = typeof(CanBuildMachine);
                    break;

                case "CirclePlacementObj":
                    type = typeof(CirclePlacementObj);
                    break;

                case "CompassWithNavigator":
                    type = typeof(CompassWithNavigator);
                    break;

                case "CoopBossAppearDemoLocator":
                    type = typeof(CoopBossAppearDemoLocator);
                    break;

                case "CoopBossAppearDemoLocatorSakeJaw":
                    type = typeof(CoopBossAppearDemoLocatorSakeJaw);
                    break;

                case "CoopBossAppearDemoLocatorSakeTriple":
                    type = typeof(CoopBossAppearDemoLocatorSakeTriple);
                    break;

                case "CoopEventRelayGoldenIkuraDropPoint0":
                    type = typeof(CoopEventRelayGoldenIkuraDropPoint0);
                    break;

                case "CoopEventRelayGoldenIkuraDropPoint1":
                    type = typeof(CoopEventRelayGoldenIkuraDropPoint1);
                    break;

                case "CoopEventRelayGoldenIkuraDropPoint2":
                    type = typeof(CoopEventRelayGoldenIkuraDropPoint2);
                    break;

                case "CoopGoldenIkuraDropCorrectArea":
                    type = typeof(CoopGoldenIkuraDropCorrectArea);
                    break;

                case "CoopGoldenIkuraReserveDropPos":
                    type = typeof(CoopGoldenIkuraReserveDropPos);
                    break;

                case "CoopGoldenIkuraUnbindArea":
                    type = typeof(CoopGoldenIkuraUnbindArea);
                    break;

                case "CoopHelicopterCenterPoint":
                    type = typeof(CoopHelicopterCenterPoint);
                    break;

                case "CoopParamHolderEventGeyser":
                    type = typeof(CoopParamHolderEventGeyser);
                    break;

                case "CoopPathCorrectArea":
                    type = typeof(CoopPathCorrectArea);
                    break;

                case "CoopSakeBigMouthNoDropIkuraArea":
                    type = typeof(CoopSakeBigMouthNoDropIkuraArea);
                    break;

                case "CoopSakeFlyBagManArrivalPointForLift":
                    type = typeof(CoopSakeFlyBagManArrivalPointForLift);
                    break;

                case "CoopSakePillarSpawnPoint0":
                    type = typeof(CoopSakePillarSpawnPoint0);
                    break;

                case "CoopSakePillarSpawnPoint1":
                    type = typeof(CoopSakePillarSpawnPoint1);
                    break;

                case "CoopSakePillarSpawnPoint2":
                    type = typeof(CoopSakePillarSpawnPoint2);
                    break;

                case "CoopSakerocketJumpPoint":
                    type = typeof(CoopSakerocketJumpPoint);
                    break;

                case "CoopSpawnGeyser":
                    type = typeof(CoopSpawnGeyser);
                    break;

                case "CoopSpawnPointEnemy0":
                    type = typeof(CoopSpawnPointEnemy0);
                    break;

                case "CoopSpawnPointEnemy1":
                    type = typeof(CoopSpawnPointEnemy1);
                    break;

                case "CoopSpawnPointEnemy2":
                    type = typeof(CoopSpawnPointEnemy2);
                    break;

                case "CullingArea":
                    type = typeof(CullingArea);
                    break;

                case "CullingAreaSubordinate":
                    type = typeof(CullingAreaSubordinate);
                    break;

                case "CullingOcclusionArea":
                    type = typeof(CullingOcclusionArea);
                    break;

                case "D":
                    type = typeof(D);
                    break;

                case "DamageConveyor":
                    type = typeof(DamageConveyor);
                    break;

                case "DashPanel10":
                    type = typeof(DashPanel10);
                    break;

                case "DashPanel10Ice":
                    type = typeof(DashPanel10Ice);
                    break;

                case "DashPanel10Sdodr":
                    type = typeof(DashPanel10Sdodr);
                    break;

                case "DashPanel30":
                    type = typeof(DashPanel30);
                    break;

                case "DashPanel30Sdodr":
                    type = typeof(DashPanel30Sdodr);
                    break;

                case "DashPanelVertical":
                    type = typeof(DashPanelVertical);
                    break;

                case "DashPanelVerticalSdodr":
                    type = typeof(DashPanelVerticalSdodr);
                    break;

                case "DashPanelVerticalStrong":
                    type = typeof(DashPanelVerticalStrong);
                    break;

                case "DashPanelVerticalStrongSdodr":
                    type = typeof(DashPanelVerticalStrongSdodr);
                    break;

                case "DashPanelWall":
                    type = typeof(DashPanelWall);
                    break;

                case "DD":
                    type = typeof(DD);
                    break;

                case "DemoActorA":
                    type = typeof(DemoActorA);
                    break;

                case "DemoLight2":
                    type = typeof(DemoLight2);
                    break;

                case "DemoLightA":
                    type = typeof(DemoLightA);
                    break;

                case "DemoLightB":
                    type = typeof(DemoLightB);
                    break;

                case "DemoLightC":
                    type = typeof(DemoLightC);
                    break;

                case "DemoLight１":
                    type = typeof(DemoLight１);
                    break;

                case "DObj_Aurora":
                    type = typeof(DObj_Aurora);
                    break;

                case "DObj_AutoWalk00_BankaraTrain":
                    type = typeof(DObj_AutoWalk00_BankaraTrain);
                    break;

                case "DObj_AutoWalk00_Cars":
                    type = typeof(DObj_AutoWalk00_Cars);
                    break;

                case "DObj_AutoWalk00_Fence":
                    type = typeof(DObj_AutoWalk00_Fence);
                    break;

                case "DObj_AutoWalk00_Signage":
                    type = typeof(DObj_AutoWalk00_Signage);
                    break;

                case "DObj_BankaraCityFar":
                    type = typeof(DObj_BankaraCityFar);
                    break;

                case "DObj_BeaconLightCoop":
                    type = typeof(DObj_BeaconLightCoop);
                    break;

                case "DObj_BigNamazu":
                    type = typeof(DObj_BigNamazu);
                    break;

                case "DObj_BoatSteamerA":
                    type = typeof(DObj_BoatSteamerA);
                    break;

                case "DObj_Bunker01":
                    type = typeof(DObj_Bunker01);
                    break;

                case "DObj_Bunker02":
                    type = typeof(DObj_Bunker02);
                    break;

                case "DObj_BunkerCliff01":
                    type = typeof(DObj_BunkerCliff01);
                    break;

                case "DObj_BunkerCliff02":
                    type = typeof(DObj_BunkerCliff02);
                    break;

                case "DObj_BunkerHalfWall01":
                    type = typeof(DObj_BunkerHalfWall01);
                    break;

                case "DObj_BunkerHalfWall02":
                    type = typeof(DObj_BunkerHalfWall02);
                    break;

                case "DObj_BunkerMsn01":
                    type = typeof(DObj_BunkerMsn01);
                    break;

                case "DObj_BunkerMsn02":
                    type = typeof(DObj_BunkerMsn02);
                    break;

                case "DObj_BunkerTowerKingSdodr":
                    type = typeof(DObj_BunkerTowerKingSdodr);
                    break;

                case "DObj_BunkerWall01":
                    type = typeof(DObj_BunkerWall01);
                    break;

                case "DObj_BunkerWall02":
                    type = typeof(DObj_BunkerWall02);
                    break;

                case "DObj_Cage":
                    type = typeof(DObj_Cage);
                    break;

                case "DObj_CarouselFreeFall":
                    type = typeof(DObj_CarouselFreeFall);
                    break;

                case "DObj_CarouselJetCoaster":
                    type = typeof(DObj_CarouselJetCoaster);
                    break;

                case "DObj_CarouselMerryGoRound":
                    type = typeof(DObj_CarouselMerryGoRound);
                    break;

                case "DObj_ChromaticAberration":
                    type = typeof(DObj_ChromaticAberration);
                    break;

                case "DObj_Collector_Sdodr":
                    type = typeof(DObj_Collector_Sdodr);
                    break;

                case "DObj_CopSectionLightShaft":
                    type = typeof(DObj_CopSectionLightShaft);
                    break;

                case "DObj_CraterTent":
                    type = typeof(DObj_CraterTent);
                    break;

                case "DObj_Cross00_CookingRobot":
                    type = typeof(DObj_Cross00_CookingRobot);
                    break;

                case "DObj_Cross00_CookingRobot_PntVarVcl":
                    type = typeof(DObj_Cross00_CookingRobot_PntVarVcl);
                    break;

                case "DObj_Cross00_CookingRobot_Tcl":
                    type = typeof(DObj_Cross00_CookingRobot_Tcl);
                    break;

                case "DObj_Cross00_CookingRobot_Vlf":
                    type = typeof(DObj_Cross00_CookingRobot_Vlf);
                    break;

                case "DObj_Cross00_DeliveryRobotAndCar":
                    type = typeof(DObj_Cross00_DeliveryRobotAndCar);
                    break;

                case "DObj_Cross00_Monitor":
                    type = typeof(DObj_Cross00_Monitor);
                    break;

                case "DObj_Cross00_RamenChopstick":
                    type = typeof(DObj_Cross00_RamenChopstick);
                    break;

                case "DObj_D5_01CombineBG":
                    type = typeof(DObj_D5_01CombineBG);
                    break;

                case "DObj_D6_04AlternaBG":
                    type = typeof(DObj_D6_04AlternaBG);
                    break;

                case "DObj_DemoBlade":
                    type = typeof(DObj_DemoBlade);
                    break;

                case "DObj_DemoFrame":
                    type = typeof(DObj_DemoFrame);
                    break;

                case "DObj_District00_MonoA":
                    type = typeof(DObj_District00_MonoA);
                    break;

                case "DObj_District00_MonoB":
                    type = typeof(DObj_District00_MonoB);
                    break;

                case "DObj_DObj_FesTeamGoods":
                    type = typeof(DObj_DObj_FesTeamGoods);
                    break;

                case "DObj_DObj_Shakehighway00":
                    type = typeof(DObj_DObj_Shakehighway00);
                    break;

                case "DObj_DObj_ShakehighwayRopeLight":
                    type = typeof(DObj_DObj_ShakehighwayRopeLight);
                    break;

                case "DObj_EarthLastBoss":
                    type = typeof(DObj_EarthLastBoss);
                    break;

                case "DObj_ExhaustFan":
                    type = typeof(DObj_ExhaustFan);
                    break;

                case "DObj_FldBG_Elevator00":
                    type = typeof(DObj_FldBG_Elevator00);
                    break;

                case "DObj_FldBG_Ruins03_Boat":
                    type = typeof(DObj_FldBG_Ruins03_Boat);
                    break;

                case "DObj_FldBG_Ruins03_CruiseShip":
                    type = typeof(DObj_FldBG_Ruins03_CruiseShip);
                    break;

                case "DObj_FldBG_Ruins03_FishingBoat":
                    type = typeof(DObj_FldBG_Ruins03_FishingBoat);
                    break;

                case "DObj_FldBG_Ruins03_Float":
                    type = typeof(DObj_FldBG_Ruins03_Float);
                    break;

                case "DObj_FldBG_Ruins03_Light":
                    type = typeof(DObj_FldBG_Ruins03_Light);
                    break;

                case "DObj_FldBG_Ruins03_Tanker":
                    type = typeof(DObj_FldBG_Ruins03_Tanker);
                    break;

                case "DObj_FldBG_Section002DJerry":
                    type = typeof(DObj_FldBG_Section002DJerry);
                    break;

                case "DObj_FldBG_Section00Escalator":
                    type = typeof(DObj_FldBG_Section00Escalator);
                    break;

                case "DObj_FldBG_Section012DJerry":
                    type = typeof(DObj_FldBG_Section012DJerry);
                    break;

                case "DObj_FldBG_Section01Escalator":
                    type = typeof(DObj_FldBG_Section01Escalator);
                    break;

                case "DObj_FldBG_Spider00_Sea":
                    type = typeof(DObj_FldBG_Spider00_Sea);
                    break;

                case "DObj_FldObj_BigRunUpland03_Water":
                    type = typeof(DObj_FldObj_BigRunUpland03_Water);
                    break;

                case "DObj_FldObj_BlueCranes":
                    type = typeof(DObj_FldObj_BlueCranes);
                    break;

                case "DObj_FldObj_CarouselRotationAttractions":
                    type = typeof(DObj_FldObj_CarouselRotationAttractions);
                    break;

                case "DObj_FldObj_CeilingLight":
                    type = typeof(DObj_FldObj_CeilingLight);
                    break;

                case "DObj_FldObj_CornerLotus":
                    type = typeof(DObj_FldObj_CornerLotus);
                    break;

                case "DObj_FldObj_CurbLight":
                    type = typeof(DObj_FldObj_CurbLight);
                    break;

                case "DObj_FldObj_CurbLight2nd":
                    type = typeof(DObj_FldObj_CurbLight2nd);
                    break;

                case "DObj_FldObj_DoorVSLobbyLocker":
                    type = typeof(DObj_FldObj_DoorVSLobbyLocker);
                    break;

                case "DObj_FldObj_Factory00StoneStatueNight":
                    type = typeof(DObj_FldObj_Factory00StoneStatueNight);
                    break;

                case "DObj_FldObj_Hiagari04FloatBallL":
                    type = typeof(DObj_FldObj_Hiagari04FloatBallL);
                    break;

                case "DObj_FldObj_Hiagari04FloatBallM":
                    type = typeof(DObj_FldObj_Hiagari04FloatBallM);
                    break;

                case "DObj_FldObj_Hiagari04FloatBallS":
                    type = typeof(DObj_FldObj_Hiagari04FloatBallS);
                    break;

                case "DObj_FldObj_Hiagari04FloatDonut":
                    type = typeof(DObj_FldObj_Hiagari04FloatDonut);
                    break;

                case "DObj_FldObj_Hiagari04FloatMattressA":
                    type = typeof(DObj_FldObj_Hiagari04FloatMattressA);
                    break;

                case "DObj_FldObj_Hiagari04FloatMattressB":
                    type = typeof(DObj_FldObj_Hiagari04FloatMattressB);
                    break;

                case "DObj_FldObj_Hiagari04FloatSlide":
                    type = typeof(DObj_FldObj_Hiagari04FloatSlide);
                    break;

                case "DObj_FldObj_HiagariFloatBallL":
                    type = typeof(DObj_FldObj_HiagariFloatBallL);
                    break;

                case "DObj_FldObj_HiagariFloatBallM":
                    type = typeof(DObj_FldObj_HiagariFloatBallM);
                    break;

                case "DObj_FldObj_HiagariFloatBallS":
                    type = typeof(DObj_FldObj_HiagariFloatBallS);
                    break;

                case "DObj_FldObj_HiagariFloatDonut":
                    type = typeof(DObj_FldObj_HiagariFloatDonut);
                    break;

                case "DObj_FldObj_HiagariFloatMattressA":
                    type = typeof(DObj_FldObj_HiagariFloatMattressA);
                    break;

                case "DObj_FldObj_HiagariFloatMattressB":
                    type = typeof(DObj_FldObj_HiagariFloatMattressB);
                    break;

                case "DObj_FldObj_HiagariFloatSlide":
                    type = typeof(DObj_FldObj_HiagariFloatSlide);
                    break;

                case "DObj_FldObj_JyohekiPlayGroundSeesaw":
                    type = typeof(DObj_FldObj_JyohekiPlayGroundSeesaw);
                    break;

                case "DObj_FldObj_Kaisou03Fest":
                    type = typeof(DObj_FldObj_Kaisou03Fest);
                    break;

                case "DObj_FldObj_Kaisou04Fest":
                    type = typeof(DObj_FldObj_Kaisou04Fest);
                    break;

                case "DObj_FldObj_KumaRocketBrokenDebris":
                    type = typeof(DObj_FldObj_KumaRocketBrokenDebris);
                    break;

                case "DObj_FldObj_KumaRocketDebris":
                    type = typeof(DObj_FldObj_KumaRocketDebris);
                    break;

                case "DObj_FldObj_Lotus":
                    type = typeof(DObj_FldObj_Lotus);
                    break;

                case "DObj_FldObj_MsnSharkKingCage":
                    type = typeof(DObj_FldObj_MsnSharkKingCage);
                    break;

                case "DObj_FldObj_MsnSharkKingCoin":
                    type = typeof(DObj_FldObj_MsnSharkKingCoin);
                    break;

                case "DObj_FldObj_PalmTreeBall":
                    type = typeof(DObj_FldObj_PalmTreeBall);
                    break;

                case "DObj_FldObj_PillerLight":
                    type = typeof(DObj_FldObj_PillerLight);
                    break;

                case "DObj_FldObj_Propeller00Bass":
                    type = typeof(DObj_FldObj_Propeller00Bass);
                    break;

                case "DObj_FldObj_Propeller00StandardPlane":
                    type = typeof(DObj_FldObj_Propeller00StandardPlane);
                    break;

                case "DObj_FldObj_Propeller00StandardPlaneFar":
                    type = typeof(DObj_FldObj_Propeller00StandardPlaneFar);
                    break;

                case "DObj_FldObj_Propeller00Vehicle":
                    type = typeof(DObj_FldObj_Propeller00Vehicle);
                    break;

                case "DObj_FldObj_SdodrElevatorShaftLight":
                    type = typeof(DObj_FldObj_SdodrElevatorShaftLight);
                    break;

                case "DObj_FldObj_SdodrPlazaCraneGameMachine":
                    type = typeof(DObj_FldObj_SdodrPlazaCraneGameMachine);
                    break;

                case "DObj_FldObj_SdodrPlazaMirrorBall":
                    type = typeof(DObj_FldObj_SdodrPlazaMirrorBall);
                    break;

                case "DObj_FldObj_SdodrPlazaOrderExtraShopTarcho":
                    type = typeof(DObj_FldObj_SdodrPlazaOrderExtraShopTarcho);
                    break;

                case "DObj_FldObj_SdodrPlazaPartsBigRun":
                    type = typeof(DObj_FldObj_SdodrPlazaPartsBigRun);
                    break;

                case "DObj_FldObj_SdodrPlazaPartsDay":
                    type = typeof(DObj_FldObj_SdodrPlazaPartsDay);
                    break;

                case "DObj_FldObj_SdodrPlazaPartsFest":
                    type = typeof(DObj_FldObj_SdodrPlazaPartsFest);
                    break;

                case "DObj_FldObj_SdodrPlazaSignageScroll":
                    type = typeof(DObj_FldObj_SdodrPlazaSignageScroll);
                    break;

                case "DObj_FldObj_SdodrPlazaStageBegin":
                    type = typeof(DObj_FldObj_SdodrPlazaStageBegin);
                    break;

                case "DObj_FldObj_SdodrPlazaStageEnd":
                    type = typeof(DObj_FldObj_SdodrPlazaStageEnd);
                    break;

                case "DObj_FldObj_SdodrPlazaStageSpeakerCenter":
                    type = typeof(DObj_FldObj_SdodrPlazaStageSpeakerCenter);
                    break;

                case "DObj_FldObj_SdodrPole":
                    type = typeof(DObj_FldObj_SdodrPole);
                    break;

                case "DObj_FldObj_SdodrStaffCreditBackground":
                    type = typeof(DObj_FldObj_SdodrStaffCreditBackground);
                    break;

                case "DObj_FldObj_SdodrStaffrollCloud":
                    type = typeof(DObj_FldObj_SdodrStaffrollCloud);
                    break;

                case "DObj_FldObj_Sdodr_StaffRollVirtualRealityDisappear":
                    type = typeof(DObj_FldObj_Sdodr_StaffRollVirtualRealityDisappear);
                    break;

                case "DObj_FldObj_SearchLights":
                    type = typeof(DObj_FldObj_SearchLights);
                    break;

                case "DObj_FldObj_Section00_Signage":
                    type = typeof(DObj_FldObj_Section00_Signage);
                    break;

                case "DObj_FldObj_Section00_SignagePnt":
                    type = typeof(DObj_FldObj_Section00_SignagePnt);
                    break;

                case "DObj_FldObj_Section00_SignageTcl":
                    type = typeof(DObj_FldObj_Section00_SignageTcl);
                    break;

                case "DObj_FldObj_Section00_SignageVgl":
                    type = typeof(DObj_FldObj_Section00_SignageVgl);
                    break;

                case "DObj_FldObj_Section00_Water":
                    type = typeof(DObj_FldObj_Section00_Water);
                    break;

                case "DObj_FldObj_Section01_PalmTreeBall":
                    type = typeof(DObj_FldObj_Section01_PalmTreeBall);
                    break;

                case "DObj_FldObj_Section01_SignageCmn":
                    type = typeof(DObj_FldObj_Section01_SignageCmn);
                    break;

                case "DObj_FldObj_Section01_SignagePntVar":
                    type = typeof(DObj_FldObj_Section01_SignagePntVar);
                    break;

                case "DObj_FldObj_Section01_SignageVlf":
                    type = typeof(DObj_FldObj_Section01_SignageVlf);
                    break;

                case "DObj_FldObj_Section01_Water":
                    type = typeof(DObj_FldObj_Section01_Water);
                    break;

                case "DObj_FldObj_Shakerail00BG2DShake":
                    type = typeof(DObj_FldObj_Shakerail00BG2DShake);
                    break;

                case "DObj_FldObj_Spider00_Elevator":
                    type = typeof(DObj_FldObj_Spider00_Elevator);
                    break;

                case "DObj_FldObj_Spider00_RightSign":
                    type = typeof(DObj_FldObj_Spider00_RightSign);
                    break;

                case "DObj_FldObj_Spider00_VlfChargerYoke":
                    type = typeof(DObj_FldObj_Spider00_VlfChargerYoke);
                    break;

                case "DObj_FldObj_Spider00_VlfElevator":
                    type = typeof(DObj_FldObj_Spider00_VlfElevator);
                    break;

                case "DObj_FldObj_SpiderWindsock":
                    type = typeof(DObj_FldObj_SpiderWindsock);
                    break;

                case "DObj_FldObj_TransparentActor":
                    type = typeof(DObj_FldObj_TransparentActor);
                    break;

                case "DObj_FldObj_TransparentActorLarge":
                    type = typeof(DObj_FldObj_TransparentActorLarge);
                    break;

                case "DObj_FldObj_Upland03_Car":
                    type = typeof(DObj_FldObj_Upland03_Car);
                    break;

                case "DObj_FldObj_YagaraFloatBoatA":
                    type = typeof(DObj_FldObj_YagaraFloatBoatA);
                    break;

                case "DObj_FldObj_YagaraFloatBoatB":
                    type = typeof(DObj_FldObj_YagaraFloatBoatB);
                    break;

                case "DObj_FldObj_YagaraFloatBuoyA":
                    type = typeof(DObj_FldObj_YagaraFloatBuoyA);
                    break;

                case "DObj_FldObj_YagaraFloatDebrisA":
                    type = typeof(DObj_FldObj_YagaraFloatDebrisA);
                    break;

                case "DObj_FldObj_YagaraFloatDebrisB":
                    type = typeof(DObj_FldObj_YagaraFloatDebrisB);
                    break;

                case "DObj_FldObj_YagaraFloatDebrisC":
                    type = typeof(DObj_FldObj_YagaraFloatDebrisC);
                    break;

                case "DObj_FldObj_YagaraFloatDebrisD":
                    type = typeof(DObj_FldObj_YagaraFloatDebrisD);
                    break;

                case "DObj_FldObj_YagaraFloatDebrisE":
                    type = typeof(DObj_FldObj_YagaraFloatDebrisE);
                    break;

                case "DObj_FldObj_YagaraFloatDebrisF":
                    type = typeof(DObj_FldObj_YagaraFloatDebrisF);
                    break;

                case "DObj_FldObj_YagaraFloatDebrisG":
                    type = typeof(DObj_FldObj_YagaraFloatDebrisG);
                    break;

                case "DObj_Fld_BankaraCityVehicle":
                    type = typeof(DObj_Fld_BankaraCityVehicle);
                    break;

                case "DObj_Fld_BankaraCityVehicleFest":
                    type = typeof(DObj_Fld_BankaraCityVehicleFest);
                    break;

                case "DObj_Fld_BankaraCityVehiclePlane":
                    type = typeof(DObj_Fld_BankaraCityVehiclePlane);
                    break;

                case "DObj_Fld_BigWorldRocket":
                    type = typeof(DObj_Fld_BigWorldRocket);
                    break;

                case "DObj_Fld_BigWorldRocketAndLaunchPad":
                    type = typeof(DObj_Fld_BigWorldRocketAndLaunchPad);
                    break;

                case "DObj_Fld_BigWorldRocketAndLaunchPadAfter":
                    type = typeof(DObj_Fld_BigWorldRocketAndLaunchPadAfter);
                    break;

                case "DObj_Fld_FesProjectorBox":
                    type = typeof(DObj_Fld_FesProjectorBox);
                    break;

                case "DObj_Fld_PlazaSanpoSignboard":
                    type = typeof(DObj_Fld_PlazaSanpoSignboard);
                    break;

                case "DObj_Fld_PlazaSignageFest":
                    type = typeof(DObj_Fld_PlazaSignageFest);
                    break;

                case "DObj_Fld_PlazaSignageFestNoStage":
                    type = typeof(DObj_Fld_PlazaSignageFestNoStage);
                    break;

                case "DObj_Fld_RocketSiloHole":
                    type = typeof(DObj_Fld_RocketSiloHole);
                    break;

                case "DObj_Fld_SdodrBarrierKingOdako1stSignage":
                    type = typeof(DObj_Fld_SdodrBarrierKingOdako1stSignage);
                    break;

                case "DObj_Fld_SdodrBarrierKingOdako1stTakotsubo":
                    type = typeof(DObj_Fld_SdodrBarrierKingOdako1stTakotsubo);
                    break;

                case "DObj_Fld_SdodrBarrierKingOdako2ndBG":
                    type = typeof(DObj_Fld_SdodrBarrierKingOdako2ndBG);
                    break;

                case "DObj_Fld_SdodrBarrierKingOdako2ndSignage":
                    type = typeof(DObj_Fld_SdodrBarrierKingOdako2ndSignage);
                    break;

                case "DObj_Fld_SdodrDJboothBarrier":
                    type = typeof(DObj_Fld_SdodrDJboothBarrier);
                    break;

                case "DObj_Fld_SdodrDJboothBarrier02":
                    type = typeof(DObj_Fld_SdodrDJboothBarrier02);
                    break;

                case "DObj_Fld_SdodrElevatorDoor":
                    type = typeof(DObj_Fld_SdodrElevatorDoor);
                    break;

                case "DObj_Fld_SdodrOdakoDJBooth2ndEX":
                    type = typeof(DObj_Fld_SdodrOdakoDJBooth2ndEX);
                    break;

                case "DObj_Fld_SdodrStaffRollDebris":
                    type = typeof(DObj_Fld_SdodrStaffRollDebris);
                    break;

                case "DObj_Fld_Sdodr_LogoutElevator":
                    type = typeof(DObj_Fld_Sdodr_LogoutElevator);
                    break;

                case "DObj_Fld_Sdodr_LogoutElevatorTruss":
                    type = typeof(DObj_Fld_Sdodr_LogoutElevatorTruss);
                    break;

                case "DObj_Fld_SmallWorldWaterStorageTank":
                    type = typeof(DObj_Fld_SmallWorldWaterStorageTank);
                    break;

                case "DObj_Fld_SmallWorldWindmill":
                    type = typeof(DObj_Fld_SmallWorldWindmill);
                    break;

                case "DObj_GantryCrane":
                    type = typeof(DObj_GantryCrane);
                    break;

                case "DObj_Guillotine":
                    type = typeof(DObj_Guillotine);
                    break;

                case "DObj_KebaInkC6":
                    type = typeof(DObj_KebaInkC6);
                    break;

                case "DObj_KebaInkC6Appear":
                    type = typeof(DObj_KebaInkC6Appear);
                    break;

                case "DObj_LobbyCapsuleMachineFest":
                    type = typeof(DObj_LobbyCapsuleMachineFest);
                    break;

                case "DObj_Minigame":
                    type = typeof(DObj_Minigame);
                    break;

                case "DObj_MinigameChairBlue":
                    type = typeof(DObj_MinigameChairBlue);
                    break;

                case "DObj_MinigameChairYellow":
                    type = typeof(DObj_MinigameChairYellow);
                    break;

                case "DObj_MorayChain":
                    type = typeof(DObj_MorayChain);
                    break;

                case "DObj_MsnMorayKingSign":
                    type = typeof(DObj_MsnMorayKingSign);
                    break;

                case "DObj_MsnMorayKingWave":
                    type = typeof(DObj_MsnMorayKingWave);
                    break;

                case "DObj_Npc_FishHead":
                    type = typeof(DObj_Npc_FishHead);
                    break;

                case "DObj_Npc_JerryPlayerMake":
                    type = typeof(DObj_Npc_JerryPlayerMake);
                    break;

                case "DObj_Npc_MobA_Sdodr":
                    type = typeof(DObj_Npc_MobA_Sdodr);
                    break;

                case "DObj_Npc_MobA_Sdodr_Demo1":
                    type = typeof(DObj_Npc_MobA_Sdodr_Demo1);
                    break;

                case "DObj_Npc_MobA_Sdodr_Demo2":
                    type = typeof(DObj_Npc_MobA_Sdodr_Demo2);
                    break;

                case "DObj_Npc_MobB_Sdodr":
                    type = typeof(DObj_Npc_MobB_Sdodr);
                    break;

                case "DObj_Npc_MobC_Sdodr":
                    type = typeof(DObj_Npc_MobC_Sdodr);
                    break;

                case "DObj_Npc_MobC_Wall_Sdodr":
                    type = typeof(DObj_Npc_MobC_Wall_Sdodr);
                    break;

                case "DObj_Npc_MobE_Sdodr":
                    type = typeof(DObj_Npc_MobE_Sdodr);
                    break;

                case "DObj_Npc_MobF_Sdodr":
                    type = typeof(DObj_Npc_MobF_Sdodr);
                    break;

                case "DObj_Obj_DitchTree":
                    type = typeof(DObj_Obj_DitchTree);
                    break;

                case "DObj_Obj_SoundTree00":
                    type = typeof(DObj_Obj_SoundTree00);
                    break;

                case "DObj_PhayaoAppleCoop":
                    type = typeof(DObj_PhayaoAppleCoop);
                    break;

                case "DObj_PhayaoKiwiCoop":
                    type = typeof(DObj_PhayaoKiwiCoop);
                    break;

                case "DObj_PhayaoMangoCoop":
                    type = typeof(DObj_PhayaoMangoCoop);
                    break;

                case "DObj_PlaneFog":
                    type = typeof(DObj_PlaneFog);
                    break;

                case "DObj_PlaneFog0":
                    type = typeof(DObj_PlaneFog0);
                    break;

                case "DObj_PlaneFog1":
                    type = typeof(DObj_PlaneFog1);
                    break;

                case "DObj_PlaneFog2":
                    type = typeof(DObj_PlaneFog2);
                    break;

                case "DObj_PlaneFog3":
                    type = typeof(DObj_PlaneFog3);
                    break;

                case "DObj_PlayerMakeTrainInside":
                    type = typeof(DObj_PlayerMakeTrainInside);
                    break;

                case "DObj_PlayerMakeTrainOutside":
                    type = typeof(DObj_PlayerMakeTrainOutside);
                    break;

                case "DObj_PlazaHelicopter":
                    type = typeof(DObj_PlazaHelicopter);
                    break;

                case "DObj_PlazaHelicopterHovering":
                    type = typeof(DObj_PlazaHelicopterHovering);
                    break;

                case "DObj_PlazaHelicopterTurning":
                    type = typeof(DObj_PlazaHelicopterTurning);
                    break;

                case "DObj_PlazaJerry_Zakka00_Fsodr":
                    type = typeof(DObj_PlazaJerry_Zakka00_Fsodr);
                    break;

                case "DObj_PlazaJerry_Zakka01_Fsodr":
                    type = typeof(DObj_PlazaJerry_Zakka01_Fsodr);
                    break;

                case "DObj_PlazaMiniGameDecorationBoard":
                    type = typeof(DObj_PlazaMiniGameDecorationBoard);
                    break;

                case "DObj_PlazaMiniGameDecorationFlag":
                    type = typeof(DObj_PlazaMiniGameDecorationFlag);
                    break;

                case "DObj_PlazaTurbanshellsCase":
                    type = typeof(DObj_PlazaTurbanshellsCase);
                    break;

                case "DObj_PlazaTurbanshellsCaseClose":
                    type = typeof(DObj_PlazaTurbanshellsCaseClose);
                    break;

                case "DObj_Prop_PaperWallTower":
                    type = typeof(DObj_Prop_PaperWallTower);
                    break;

                case "DObj_Razor":
                    type = typeof(DObj_Razor);
                    break;

                case "DObj_Razor_Broken":
                    type = typeof(DObj_Razor_Broken);
                    break;

                case "DObj_SalmonBouyAsparagus":
                    type = typeof(DObj_SalmonBouyAsparagus);
                    break;

                case "DObj_SalmonBouyCorn":
                    type = typeof(DObj_SalmonBouyCorn);
                    break;

                case "DObj_SalmonBouySausage":
                    type = typeof(DObj_SalmonBouySausage);
                    break;

                case "DObj_SalmonPan":
                    type = typeof(DObj_SalmonPan);
                    break;

                case "DObj_SalmonTire":
                    type = typeof(DObj_SalmonTire);
                    break;

                case "DObj_SalmonWoodchip":
                    type = typeof(DObj_SalmonWoodchip);
                    break;

                case "DObj_SchoolOfFishMoray_01":
                    type = typeof(DObj_SchoolOfFishMoray_01);
                    break;

                case "DObj_SchoolOfFishMoray_02":
                    type = typeof(DObj_SchoolOfFishMoray_02);
                    break;

                case "DObj_SchoolOfFishMoray_03":
                    type = typeof(DObj_SchoolOfFishMoray_03);
                    break;

                case "DObj_Scrap01GantryCrane":
                    type = typeof(DObj_Scrap01GantryCrane);
                    break;

                case "DObj_Scrap01Guillotine":
                    type = typeof(DObj_Scrap01Guillotine);
                    break;

                case "DObj_Scrap01VehicleForkLift":
                    type = typeof(DObj_Scrap01VehicleForkLift);
                    break;

                case "DObj_SdodrCubeAnimation":
                    type = typeof(DObj_SdodrCubeAnimation);
                    break;

                case "DObj_SdodrEffectLocatorA":
                    type = typeof(DObj_SdodrEffectLocatorA);
                    break;

                case "DObj_SdodrEffectLocatorB":
                    type = typeof(DObj_SdodrEffectLocatorB);
                    break;

                case "DObj_SdodrEffectLocatorC":
                    type = typeof(DObj_SdodrEffectLocatorC);
                    break;

                case "DObj_SdodrEffectLocatorD":
                    type = typeof(DObj_SdodrEffectLocatorD);
                    break;

                case "DObj_SdodrEffectLocatorE":
                    type = typeof(DObj_SdodrEffectLocatorE);
                    break;

                case "DObj_SdodrOdakoDJBooth00":
                    type = typeof(DObj_SdodrOdakoDJBooth00);
                    break;

                case "DObj_SdodrPlazaDanceMachine":
                    type = typeof(DObj_SdodrPlazaDanceMachine);
                    break;

                case "DObj_SdodrPlazaHelicopter":
                    type = typeof(DObj_SdodrPlazaHelicopter);
                    break;

                case "DObj_SdodrPlazaHelicopterHovering":
                    type = typeof(DObj_SdodrPlazaHelicopterHovering);
                    break;

                case "DObj_SdodrPlazaHelicopterTurning":
                    type = typeof(DObj_SdodrPlazaHelicopterTurning);
                    break;

                case "DObj_SdodrPlazaPlane":
                    type = typeof(DObj_SdodrPlazaPlane);
                    break;

                case "DObj_SdodrPlazaTrain":
                    type = typeof(DObj_SdodrPlazaTrain);
                    break;

                case "DObj_SdodrPlazaTrainBigrun":
                    type = typeof(DObj_SdodrPlazaTrainBigrun);
                    break;

                case "DObj_SdodrPlazaVehicle":
                    type = typeof(DObj_SdodrPlazaVehicle);
                    break;

                case "DObj_SdodrSardine":
                    type = typeof(DObj_SdodrSardine);
                    break;

                case "DObj_SdodrStaffCreditBoard":
                    type = typeof(DObj_SdodrStaffCreditBoard);
                    break;

                case "DObj_SdodrStaffCreditShark":
                    type = typeof(DObj_SdodrStaffCreditShark);
                    break;

                case "DObj_SdodrTowerKingErosion":
                    type = typeof(DObj_SdodrTowerKingErosion);
                    break;

                case "DObj_SdodrTuna":
                    type = typeof(DObj_SdodrTuna);
                    break;

                case "DObj_SeaBind_AluminumPot":
                    type = typeof(DObj_SeaBind_AluminumPot);
                    break;

                case "DObj_SeaBind_Buoy":
                    type = typeof(DObj_SeaBind_Buoy);
                    break;

                case "DObj_SeaBind_BuoyBurner":
                    type = typeof(DObj_SeaBind_BuoyBurner);
                    break;

                case "DObj_SeaBind_BuoyBurnerPumpkin":
                    type = typeof(DObj_SeaBind_BuoyBurnerPumpkin);
                    break;

                case "DObj_SeaBind_BuoyTripod":
                    type = typeof(DObj_SeaBind_BuoyTripod);
                    break;

                case "DObj_SeaGull":
                    type = typeof(DObj_SeaGull);
                    break;

                case "DObj_Shakelift00Flag":
                    type = typeof(DObj_Shakelift00Flag);
                    break;

                case "DObj_Shakelift00RotatingDriedFish":
                    type = typeof(DObj_Shakelift00RotatingDriedFish);
                    break;

                case "DObj_Shakelift00wire":
                    type = typeof(DObj_Shakelift00wire);
                    break;

                case "DObj_Shakeship00FloathingCrane":
                    type = typeof(DObj_Shakeship00FloathingCrane);
                    break;

                case "DObj_Shakeup00SearchLight":
                    type = typeof(DObj_Shakeup00SearchLight);
                    break;

                case "DObj_Shakeup00Ship":
                    type = typeof(DObj_Shakeup00Ship);
                    break;

                case "DObj_SurimiDai":
                    type = typeof(DObj_SurimiDai);
                    break;

                case "DObj_SurimiTVscreen":
                    type = typeof(DObj_SurimiTVscreen);
                    break;

                case "DObj_TrainInsideBack":
                    type = typeof(DObj_TrainInsideBack);
                    break;

                case "DObj_TreeSlip":
                    type = typeof(DObj_TreeSlip);
                    break;

                case "DObj_TreeSlipNoCol":
                    type = typeof(DObj_TreeSlipNoCol);
                    break;

                case "DObj_TutorialGiftBox":
                    type = typeof(DObj_TutorialGiftBox);
                    break;

                case "DObj_UplandSekko":
                    type = typeof(DObj_UplandSekko);
                    break;

                case "DObj_VehicleForkLift":
                    type = typeof(DObj_VehicleForkLift);
                    break;

                case "DObj_VssPropellerNightAirplane":
                    type = typeof(DObj_VssPropellerNightAirplane);
                    break;

                case "DObj_VssSpiderJet":
                    type = typeof(DObj_VssSpiderJet);
                    break;

                case "DObj_Waterfall":
                    type = typeof(DObj_Waterfall);
                    break;

                case "DObj_WindsockGreen":
                    type = typeof(DObj_WindsockGreen);
                    break;

                case "DummyTextArea":
                    type = typeof(DummyTextArea);
                    break;

                case "DynamicLoadingArea":
                    type = typeof(DynamicLoadingArea);
                    break;

                case "EnemyBarrierTakopter":
                    type = typeof(EnemyBarrierTakopter);
                    break;

                case "EnemyBombTakopter":
                    type = typeof(EnemyBombTakopter);
                    break;

                case "EnemyBoostBombTakopter":
                    type = typeof(EnemyBoostBombTakopter);
                    break;

                case "EnemyBoostTakopter":
                    type = typeof(EnemyBoostTakopter);
                    break;

                case "EnemyChargerTower00Sdodr":
                    type = typeof(EnemyChargerTower00Sdodr);
                    break;

                case "EnemyCleaner":
                    type = typeof(EnemyCleaner);
                    break;

                case "EnemyEscape":
                    type = typeof(EnemyEscape);
                    break;

                case "EnemyEscapeSdodr":
                    type = typeof(EnemyEscapeSdodr);
                    break;

                case "EnemyEscapeSdodr_Lift":
                    type = typeof(EnemyEscapeSdodr_Lift);
                    break;

                case "EnemyFlyingHohei":
                    type = typeof(EnemyFlyingHohei);
                    break;

                case "EnemyFlyingHoheiMother":
                    type = typeof(EnemyFlyingHoheiMother);
                    break;

                case "EnemyFlyingHoheiMotherRide":
                    type = typeof(EnemyFlyingHoheiMotherRide);
                    break;

                case "EnemyFlyingHoheiRide":
                    type = typeof(EnemyFlyingHoheiRide);
                    break;

                case "EnemyGeneratorSpectacle":
                    type = typeof(EnemyGeneratorSpectacle);
                    break;

                case "EnemyHohei":
                    type = typeof(EnemyHohei);
                    break;

                case "EnemyKumasanRocket":
                    type = typeof(EnemyKumasanRocket);
                    break;

                case "EnemyKumasanSpectacle":
                    type = typeof(EnemyKumasanSpectacle);
                    break;

                case "EnemyMantaKing":
                    type = typeof(EnemyMantaKing);
                    break;

                case "EnemyMineSpectacle":
                    type = typeof(EnemyMineSpectacle);
                    break;

                case "EnemyRock":
                    type = typeof(EnemyRock);
                    break;

                case "EnemySharkKing":
                    type = typeof(EnemySharkKing);
                    break;

                case "EnemyStamp":
                    type = typeof(EnemyStamp);
                    break;

                case "EnemyStampWave":
                    type = typeof(EnemyStampWave);
                    break;

                case "EnemyTakodozer":
                    type = typeof(EnemyTakodozer);
                    break;

                case "EnemyTakoHopper":
                    type = typeof(EnemyTakoHopper);
                    break;

                case "EnemyTakoHopperHide":
                    type = typeof(EnemyTakoHopperHide);
                    break;

                case "EnemyTakolienBoostShieldVehicle":
                    type = typeof(EnemyTakolienBoostShieldVehicle);
                    break;

                case "EnemyTakolienBoostVehicle":
                    type = typeof(EnemyTakolienBoostVehicle);
                    break;

                case "EnemyTakolienBounceShieldVehicle":
                    type = typeof(EnemyTakolienBounceShieldVehicle);
                    break;

                case "EnemyTakolienBounceVehicle":
                    type = typeof(EnemyTakolienBounceVehicle);
                    break;

                case "EnemyTakolienEasyHideVehicle":
                    type = typeof(EnemyTakolienEasyHideVehicle);
                    break;

                case "EnemyTakolienEasyShieldVehicle":
                    type = typeof(EnemyTakolienEasyShieldVehicle);
                    break;

                case "EnemyTakolienEasyVehicle":
                    type = typeof(EnemyTakolienEasyVehicle);
                    break;

                case "EnemyTakolienFixedBounceShieldVehicle":
                    type = typeof(EnemyTakolienFixedBounceShieldVehicle);
                    break;

                case "EnemyTakolienFixedBounceVehicle":
                    type = typeof(EnemyTakolienFixedBounceVehicle);
                    break;

                case "EnemyTakolienFixedEasyShieldVehicle":
                    type = typeof(EnemyTakolienFixedEasyShieldVehicle);
                    break;

                case "EnemyTakolienFixedEasyVehicle":
                    type = typeof(EnemyTakolienFixedEasyVehicle);
                    break;

                case "EnemyTakolienFixedShieldVehicle":
                    type = typeof(EnemyTakolienFixedShieldVehicle);
                    break;

                case "EnemyTakolienFixedVehicle":
                    type = typeof(EnemyTakolienFixedVehicle);
                    break;

                case "EnemyTakolienHideVehicle":
                    type = typeof(EnemyTakolienHideVehicle);
                    break;

                case "EnemyTakolienShieldVehicle":
                    type = typeof(EnemyTakolienShieldVehicle);
                    break;

                case "EnemyTakolienVehicle":
                    type = typeof(EnemyTakolienVehicle);
                    break;

                case "EnemyTakopodDEV":
                    type = typeof(EnemyTakopodDEV);
                    break;

                case "EnemyTakopter":
                    type = typeof(EnemyTakopter);
                    break;

                case "EnemyUtsubox":
                    type = typeof(EnemyUtsubox);
                    break;

                case "EventNewsDirector":
                    type = typeof(EventNewsDirector);
                    break;

                case "ExclamationMarkEmitObj":
                    type = typeof(ExclamationMarkEmitObj);
                    break;

                case "FL":
                    type = typeof(FL);
                    break;

                case "FldObj_WorldContainerSet00":
                    type = typeof(FldObj_WorldContainerSet00);
                    break;

                case "FldObj_WorldContainerSet01":
                    type = typeof(FldObj_WorldContainerSet01);
                    break;

                case "FldObj_WorldContainerSet02":
                    type = typeof(FldObj_WorldContainerSet02);
                    break;

                case "FldObj_WorldContainerSet03":
                    type = typeof(FldObj_WorldContainerSet03);
                    break;

                case "FldObj_WorldContainerSet04":
                    type = typeof(FldObj_WorldContainerSet04);
                    break;

                case "FldObj_WorldContainerSet05":
                    type = typeof(FldObj_WorldContainerSet05);
                    break;

                case "FldObj_WorldContainerSet06":
                    type = typeof(FldObj_WorldContainerSet06);
                    break;

                case "FldObj_WorldContainerSet07":
                    type = typeof(FldObj_WorldContainerSet07);
                    break;

                case "FldObj_WorldContainerSet08":
                    type = typeof(FldObj_WorldContainerSet08);
                    break;

                case "FldObj_WorldContainerSet09":
                    type = typeof(FldObj_WorldContainerSet09);
                    break;

                case "Fld_Alterna1c01":
                    type = typeof(Fld_Alterna1c01);
                    break;

                case "Fld_Alterna1c02":
                    type = typeof(Fld_Alterna1c02);
                    break;

                case "Fld_Alterna1c03":
                    type = typeof(Fld_Alterna1c03);
                    break;

                case "Fld_Alterna1c04Decoration":
                    type = typeof(Fld_Alterna1c04Decoration);
                    break;

                case "Fld_Alterna1c05":
                    type = typeof(Fld_Alterna1c05);
                    break;

                case "Fld_Alterna1c06":
                    type = typeof(Fld_Alterna1c06);
                    break;

                case "Fld_Alterna1n01":
                    type = typeof(Fld_Alterna1n01);
                    break;

                case "Fld_Alterna1n02":
                    type = typeof(Fld_Alterna1n02);
                    break;

                case "Fld_Alterna1n03":
                    type = typeof(Fld_Alterna1n03);
                    break;

                case "Fld_Alterna1n04":
                    type = typeof(Fld_Alterna1n04);
                    break;

                case "Fld_Alterna2Boss02":
                    type = typeof(Fld_Alterna2Boss02);
                    break;

                case "Fld_Alterna2c01":
                    type = typeof(Fld_Alterna2c01);
                    break;

                case "Fld_Alterna2c02":
                    type = typeof(Fld_Alterna2c02);
                    break;

                case "Fld_Alterna2c03Decoration":
                    type = typeof(Fld_Alterna2c03Decoration);
                    break;

                case "Fld_Alterna2c04":
                    type = typeof(Fld_Alterna2c04);
                    break;

                case "Fld_Alterna2c05":
                    type = typeof(Fld_Alterna2c05);
                    break;

                case "Fld_Alterna2n01":
                    type = typeof(Fld_Alterna2n01);
                    break;

                case "Fld_Alterna2n02":
                    type = typeof(Fld_Alterna2n02);
                    break;

                case "Fld_Alterna2n03":
                    type = typeof(Fld_Alterna2n03);
                    break;

                case "Fld_Alterna3c01":
                    type = typeof(Fld_Alterna3c01);
                    break;

                case "Fld_Alterna3c02":
                    type = typeof(Fld_Alterna3c02);
                    break;

                case "Fld_Alterna3c03":
                    type = typeof(Fld_Alterna3c03);
                    break;

                case "Fld_Alterna3c04":
                    type = typeof(Fld_Alterna3c04);
                    break;

                case "Fld_Alterna3n01":
                    type = typeof(Fld_Alterna3n01);
                    break;

                case "Fld_Alterna3n02":
                    type = typeof(Fld_Alterna3n02);
                    break;

                case "Fld_Alterna3n03":
                    type = typeof(Fld_Alterna3n03);
                    break;

                case "Fld_Alterna4Boss03":
                    type = typeof(Fld_Alterna4Boss03);
                    break;

                case "Fld_Alterna4c01":
                    type = typeof(Fld_Alterna4c01);
                    break;

                case "Fld_Alterna4c02Decoration":
                    type = typeof(Fld_Alterna4c02Decoration);
                    break;

                case "Fld_Alterna4c03":
                    type = typeof(Fld_Alterna4c03);
                    break;

                case "Fld_Alterna4c04":
                    type = typeof(Fld_Alterna4c04);
                    break;

                case "Fld_Alterna4c05":
                    type = typeof(Fld_Alterna4c05);
                    break;

                case "Fld_Alterna4c06":
                    type = typeof(Fld_Alterna4c06);
                    break;

                case "Fld_Alterna4c07":
                    type = typeof(Fld_Alterna4c07);
                    break;

                case "Fld_Alterna4c08":
                    type = typeof(Fld_Alterna4c08);
                    break;

                case "Fld_Alterna4c09":
                    type = typeof(Fld_Alterna4c09);
                    break;

                case "Fld_Alterna4n01":
                    type = typeof(Fld_Alterna4n01);
                    break;

                case "Fld_Alterna4n02":
                    type = typeof(Fld_Alterna4n02);
                    break;

                case "Fld_Alterna4n03":
                    type = typeof(Fld_Alterna4n03);
                    break;

                case "Fld_Alterna5c01":
                    type = typeof(Fld_Alterna5c01);
                    break;

                case "Fld_Alterna5c02":
                    type = typeof(Fld_Alterna5c02);
                    break;

                case "Fld_Alterna5c03":
                    type = typeof(Fld_Alterna5c03);
                    break;

                case "Fld_Alterna5c04":
                    type = typeof(Fld_Alterna5c04);
                    break;

                case "Fld_Alterna5c05":
                    type = typeof(Fld_Alterna5c05);
                    break;

                case "Fld_Alterna5c06":
                    type = typeof(Fld_Alterna5c06);
                    break;

                case "Fld_Alterna5c07":
                    type = typeof(Fld_Alterna5c07);
                    break;

                case "Fld_Alterna5c08":
                    type = typeof(Fld_Alterna5c08);
                    break;

                case "Fld_Alterna5c09":
                    type = typeof(Fld_Alterna5c09);
                    break;

                case "Fld_Alterna5n01":
                    type = typeof(Fld_Alterna5n01);
                    break;

                case "Fld_Alterna5n02":
                    type = typeof(Fld_Alterna5n02);
                    break;

                case "Fld_Alterna5n03":
                    type = typeof(Fld_Alterna5n03);
                    break;

                case "Fld_Alterna5n04":
                    type = typeof(Fld_Alterna5n04);
                    break;

                case "Fld_Alterna6Boss04":
                    type = typeof(Fld_Alterna6Boss04);
                    break;

                case "Fld_Alterna6c01":
                    type = typeof(Fld_Alterna6c01);
                    break;

                case "Fld_Alterna6c02":
                    type = typeof(Fld_Alterna6c02);
                    break;

                case "Fld_Alterna6c03":
                    type = typeof(Fld_Alterna6c03);
                    break;

                case "Fld_Alterna6c04Decoration":
                    type = typeof(Fld_Alterna6c04Decoration);
                    break;

                case "Fld_Alterna6c05":
                    type = typeof(Fld_Alterna6c05);
                    break;

                case "Fld_Alterna6c06":
                    type = typeof(Fld_Alterna6c06);
                    break;

                case "Fld_Alterna6c07":
                    type = typeof(Fld_Alterna6c07);
                    break;

                case "Fld_Alterna6c08":
                    type = typeof(Fld_Alterna6c08);
                    break;

                case "Fld_Alterna6c09":
                    type = typeof(Fld_Alterna6c09);
                    break;

                case "Fld_Alterna6n01":
                    type = typeof(Fld_Alterna6n01);
                    break;

                case "Fld_Alterna6n02":
                    type = typeof(Fld_Alterna6n02);
                    break;

                case "Fld_Alterna6n03":
                    type = typeof(Fld_Alterna6n03);
                    break;

                case "Fld_AlternaEX01":
                    type = typeof(Fld_AlternaEX01);
                    break;

                case "Fld_AlternaEX02":
                    type = typeof(Fld_AlternaEX02);
                    break;

                case "Fld_AlternaEX03":
                    type = typeof(Fld_AlternaEX03);
                    break;

                case "Fld_AlternaEX04":
                    type = typeof(Fld_AlternaEX04);
                    break;

                case "Fld_AlternaR01":
                    type = typeof(Fld_AlternaR01);
                    break;

                case "Fld_AlternaR02":
                    type = typeof(Fld_AlternaR02);
                    break;

                case "Fld_AlternaR03":
                    type = typeof(Fld_AlternaR03);
                    break;

                case "Fld_AlternaR04":
                    type = typeof(Fld_AlternaR04);
                    break;

                case "Fld_AlternaR05":
                    type = typeof(Fld_AlternaR05);
                    break;

                case "Fld_AreaObj01":
                    type = typeof(Fld_AreaObj01);
                    break;

                case "Fld_AreaObj02":
                    type = typeof(Fld_AreaObj02);
                    break;

                case "Fld_AreaObj03":
                    type = typeof(Fld_AreaObj03);
                    break;

                case "Fld_AreaObj04":
                    type = typeof(Fld_AreaObj04);
                    break;

                case "Fld_AreaObj05":
                    type = typeof(Fld_AreaObj05);
                    break;

                case "Fld_AreaObj06":
                    type = typeof(Fld_AreaObj06);
                    break;

                case "Fld_AutoWalk00":
                    type = typeof(Fld_AutoWalk00);
                    break;

                case "Fld_BankaraCity":
                    type = typeof(Fld_BankaraCity);
                    break;

                case "Fld_BigRunCarousel03":
                    type = typeof(Fld_BigRunCarousel03);
                    break;

                case "Fld_BigRunDistrict00":
                    type = typeof(Fld_BigRunDistrict00);
                    break;

                case "Fld_BigRunFactory00":
                    type = typeof(Fld_BigRunFactory00);
                    break;

                case "Fld_BigRunSection00":
                    type = typeof(Fld_BigRunSection00);
                    break;

                case "Fld_BigRunTemple00":
                    type = typeof(Fld_BigRunTemple00);
                    break;

                case "Fld_BigRunUpland03":
                    type = typeof(Fld_BigRunUpland03);
                    break;

                case "Fld_BigSlope00":
                    type = typeof(Fld_BigSlope00);
                    break;

                case "Fld_BigWorld_Area01":
                    type = typeof(Fld_BigWorld_Area01);
                    break;

                case "Fld_Carousel03":
                    type = typeof(Fld_Carousel03);
                    break;

                case "Fld_CoopLobby":
                    type = typeof(Fld_CoopLobby);
                    break;

                case "Fld_Crater00":
                    type = typeof(Fld_Crater00);
                    break;

                case "Fld_Crater01":
                    type = typeof(Fld_Crater01);
                    break;

                case "Fld_Crater02":
                    type = typeof(Fld_Crater02);
                    break;

                case "Fld_Crater03":
                    type = typeof(Fld_Crater03);
                    break;

                case "Fld_Cross00":
                    type = typeof(Fld_Cross00);
                    break;

                case "Fld_DemoCtaterBossLandingPoint":
                    type = typeof(Fld_DemoCtaterBossLandingPoint);
                    break;

                case "Fld_District00":
                    type = typeof(Fld_District00);
                    break;

                case "Fld_Factory00":
                    type = typeof(Fld_Factory00);
                    break;

                case "Fld_Hiagari03":
                    type = typeof(Fld_Hiagari03);
                    break;

                case "Fld_Hiagari04":
                    type = typeof(Fld_Hiagari04);
                    break;

                case "Fld_Jyoheki03":
                    type = typeof(Fld_Jyoheki03);
                    break;

                case "Fld_Kaisou03":
                    type = typeof(Fld_Kaisou03);
                    break;

                case "Fld_Kaisou04":
                    type = typeof(Fld_Kaisou04);
                    break;

                case "Fld_LaunchPadObj00":
                    type = typeof(Fld_LaunchPadObj00);
                    break;

                case "Fld_Line03":
                    type = typeof(Fld_Line03);
                    break;

                case "Fld_LocalLobby":
                    type = typeof(Fld_LocalLobby);
                    break;

                case "Fld_Nagasaki03":
                    type = typeof(Fld_Nagasaki03);
                    break;

                case "Fld_Pillar03":
                    type = typeof(Fld_Pillar03);
                    break;

                case "Fld_Pivot03":
                    type = typeof(Fld_Pivot03);
                    break;

                case "Fld_PlayerMake":
                    type = typeof(Fld_PlayerMake);
                    break;

                case "Fld_Propeller00":
                    type = typeof(Fld_Propeller00);
                    break;

                case "Fld_Ruins03":
                    type = typeof(Fld_Ruins03);
                    break;

                case "Fld_Scrap00":
                    type = typeof(Fld_Scrap00);
                    break;

                case "Fld_Scrap01":
                    type = typeof(Fld_Scrap01);
                    break;

                case "Fld_SdodrBallKing":
                    type = typeof(Fld_SdodrBallKing);
                    break;

                case "Fld_SdodrBallKing20F":
                    type = typeof(Fld_SdodrBallKing20F);
                    break;

                case "Fld_SdodrBarrierKing":
                    type = typeof(Fld_SdodrBarrierKing);
                    break;

                case "Fld_SdodrBarrierKingOdako1st":
                    type = typeof(Fld_SdodrBarrierKingOdako1st);
                    break;

                case "Fld_SdodrBarrierKingOdako1stSignage":
                    type = typeof(Fld_SdodrBarrierKingOdako1stSignage);
                    break;

                case "Fld_SdodrBarrierKingOdako1stSignage_Demo":
                    type = typeof(Fld_SdodrBarrierKingOdako1stSignage_Demo);
                    break;

                case "Fld_SdodrBarrierKingOdako2nd":
                    type = typeof(Fld_SdodrBarrierKingOdako2nd);
                    break;

                case "Fld_SdodrBarrierKingOdako2ndSignage":
                    type = typeof(Fld_SdodrBarrierKingOdako2ndSignage);
                    break;

                case "Fld_SdodrBarrierKingOdakoEXSignage":
                    type = typeof(Fld_SdodrBarrierKingOdakoEXSignage);
                    break;

                case "Fld_SdodrBarrierKingOdakoPlayField":
                    type = typeof(Fld_SdodrBarrierKingOdakoPlayField);
                    break;

                case "Fld_SdodrElevator":
                    type = typeof(Fld_SdodrElevator);
                    break;

                case "Fld_SdodrEntrance":
                    type = typeof(Fld_SdodrEntrance);
                    break;

                case "Fld_SdodrEntrance2nd":
                    type = typeof(Fld_SdodrEntrance2nd);
                    break;

                case "Fld_SdodrFloorShop":
                    type = typeof(Fld_SdodrFloorShop);
                    break;

                case "Fld_SdodrPlaza":
                    type = typeof(Fld_SdodrPlaza);
                    break;

                case "Fld_SdodrPlazaOrderFirst":
                    type = typeof(Fld_SdodrPlazaOrderFirst);
                    break;

                case "Fld_SdodrPlazaOrderSecond":
                    type = typeof(Fld_SdodrPlazaOrderSecond);
                    break;

                case "Fld_SdodrRivalKing":
                    type = typeof(Fld_SdodrRivalKing);
                    break;

                case "Fld_SdodrStaffRollBGDummyMainField":
                    type = typeof(Fld_SdodrStaffRollBGDummyMainField);
                    break;

                case "Fld_SdodrTowerKing":
                    type = typeof(Fld_SdodrTowerKing);
                    break;

                case "Fld_Section00":
                    type = typeof(Fld_Section00);
                    break;

                case "Fld_Section01":
                    type = typeof(Fld_Section01);
                    break;

                case "Fld_Shakedent00":
                    type = typeof(Fld_Shakedent00);
                    break;

                case "Fld_Shakehighway00":
                    type = typeof(Fld_Shakehighway00);
                    break;

                case "Fld_Shakelift00":
                    type = typeof(Fld_Shakelift00);
                    break;

                case "Fld_Shakerail00":
                    type = typeof(Fld_Shakerail00);
                    break;

                case "Fld_Shakeship00":
                    type = typeof(Fld_Shakeship00);
                    break;

                case "Fld_Shakespiral00":
                    type = typeof(Fld_Shakespiral00);
                    break;

                case "Fld_Shakeup00":
                    type = typeof(Fld_Shakeup00);
                    break;

                case "Fld_ShootingRange":
                    type = typeof(Fld_ShootingRange);
                    break;

                case "Fld_SmallWorld":
                    type = typeof(Fld_SmallWorld);
                    break;

                case "Fld_Spider00":
                    type = typeof(Fld_Spider00);
                    break;

                case "Fld_Temple00":
                    type = typeof(Fld_Temple00);
                    break;

                case "Fld_Temple01":
                    type = typeof(Fld_Temple01);
                    break;

                case "Fld_Triangle00":
                    type = typeof(Fld_Triangle00);
                    break;

                case "Fld_Twist00":
                    type = typeof(Fld_Twist00);
                    break;

                case "Fld_Upland03":
                    type = typeof(Fld_Upland03);
                    break;

                case "Fld_VSLobby":
                    type = typeof(Fld_VSLobby);
                    break;

                case "Fld_Wave03":
                    type = typeof(Fld_Wave03);
                    break;

                case "Fld_Yagara":
                    type = typeof(Fld_Yagara);
                    break;

                case "Fld_Yunohana":
                    type = typeof(Fld_Yunohana);
                    break;

                case "FR":
                    type = typeof(FR);
                    break;

                case "GachiasariGoal":
                    type = typeof(GachiasariGoal);
                    break;

                case "Gachihoko":
                    type = typeof(Gachihoko);
                    break;

                case "GachihokoCheckPoint":
                    type = typeof(GachihokoCheckPoint);
                    break;

                case "Gachiyagura_2M":
                    type = typeof(Gachiyagura_2M);
                    break;

                case "Gachiyagura_3M":
                    type = typeof(Gachiyagura_3M);
                    break;

                case "Gachiyagura_4M":
                    type = typeof(Gachiyagura_4M);
                    break;

                case "Geyser":
                    type = typeof(Geyser);
                    break;

                case "GeyserOnline":
                    type = typeof(GeyserOnline);
                    break;

                case "GeyserUnridable":
                    type = typeof(GeyserUnridable);
                    break;

                case "GfxAreaLightPlane":
                    type = typeof(GfxAreaLightPlane);
                    break;

                case "GfxAreaLightPlaneBGActor":
                    type = typeof(GfxAreaLightPlaneBGActor);
                    break;

                case "GfxAreaLightPlaneVSBGActor":
                    type = typeof(GfxAreaLightPlaneVSBGActor);
                    break;

                case "GfxMeshLightSphere":
                    type = typeof(GfxMeshLightSphere);
                    break;

                case "GfxPointLight":
                    type = typeof(GfxPointLight);
                    break;

                case "GfxPointLightBGActor":
                    type = typeof(GfxPointLightBGActor);
                    break;

                case "GfxPointLightDynamic":
                    type = typeof(GfxPointLightDynamic);
                    break;

                case "GfxPointLightDynamic_Building":
                    type = typeof(GfxPointLightDynamic_Building);
                    break;

                case "GfxPointLightDynamic_D040":
                    type = typeof(GfxPointLightDynamic_D040);
                    break;

                case "GfxPointLightDynamic_D050":
                    type = typeof(GfxPointLightDynamic_D050);
                    break;

                case "GfxPointLightDynamic_D060":
                    type = typeof(GfxPointLightDynamic_D060);
                    break;

                case "GfxPointLightDynamic_D090":
                    type = typeof(GfxPointLightDynamic_D090);
                    break;

                case "GfxPointLightDynamic_D100":
                    type = typeof(GfxPointLightDynamic_D100);
                    break;

                case "GfxPointLight_AirPlane":
                    type = typeof(GfxPointLight_AirPlane);
                    break;

                case "GfxPointLight_BG_":
                    type = typeof(GfxPointLight_BG_);
                    break;

                case "GfxPointLight_Building":
                    type = typeof(GfxPointLight_Building);
                    break;

                case "GfxPointLight_Cafe":
                    type = typeof(GfxPointLight_Cafe);
                    break;

                case "GfxPointLight_Info":
                    type = typeof(GfxPointLight_Info);
                    break;

                case "GfxPointLight_Inside":
                    type = typeof(GfxPointLight_Inside);
                    break;

                case "GfxPointLight_Load":
                    type = typeof(GfxPointLight_Load);
                    break;

                case "GfxPointLight_Load_Blue":
                    type = typeof(GfxPointLight_Load_Blue);
                    break;

                case "GfxPointLight_Load_Red":
                    type = typeof(GfxPointLight_Load_Red);
                    break;

                case "GfxPointLight_Parking":
                    type = typeof(GfxPointLight_Parking);
                    break;

                case "GfxPointLight_Shop":
                    type = typeof(GfxPointLight_Shop);
                    break;

                case "GfxPointLight_Street":
                    type = typeof(GfxPointLight_Street);
                    break;

                case "GfxPointLight_Warehouse":
                    type = typeof(GfxPointLight_Warehouse);
                    break;

                case "GfxSpotLight":
                    type = typeof(GfxSpotLight);
                    break;

                case "GfxSpotLightBGActor":
                    type = typeof(GfxSpotLightBGActor);
                    break;

                case "GfxSpotLightDynamic":
                    type = typeof(GfxSpotLightDynamic);
                    break;

                case "GfxSpotLightDynamic_Elevator":
                    type = typeof(GfxSpotLightDynamic_Elevator);
                    break;

                case "GfxSpotLight_Cafe":
                    type = typeof(GfxSpotLight_Cafe);
                    break;

                case "GfxSpotLight_Info":
                    type = typeof(GfxSpotLight_Info);
                    break;

                case "GfxSpotLight_Inside":
                    type = typeof(GfxSpotLight_Inside);
                    break;

                case "GfxSpotLight_Shop":
                    type = typeof(GfxSpotLight_Shop);
                    break;

                case "GreatKebaInk":
                    type = typeof(GreatKebaInk);
                    break;

                case "GrindRail":
                    type = typeof(GrindRail);
                    break;

                case "GrindRailBigWorld":
                    type = typeof(GrindRailBigWorld);
                    break;

                case "GrindRailSdodr":
                    type = typeof(GrindRailSdodr);
                    break;

                case "InkBar":
                    type = typeof(InkBar);
                    break;

                case "InkRail":
                    type = typeof(InkRail);
                    break;

                case "InkRailBigWorld":
                    type = typeof(InkRailBigWorld);
                    break;

                case "InkRailOnline":
                    type = typeof(InkRailOnline);
                    break;

                case "InkRailOnlineCoopKeepOn":
                    type = typeof(InkRailOnlineCoopKeepOn);
                    break;

                case "InkRailSdodr":
                    type = typeof(InkRailSdodr);
                    break;

                case "IntervalSpawnerForEnemyFlyingHohei":
                    type = typeof(IntervalSpawnerForEnemyFlyingHohei);
                    break;

                case "IntervalSpawnerForEnemyFlyingHoheiMother":
                    type = typeof(IntervalSpawnerForEnemyFlyingHoheiMother);
                    break;

                case "IntervalSpawnerForMovePainter":
                    type = typeof(IntervalSpawnerForMovePainter);
                    break;

                case "ItemAncientDocument":
                    type = typeof(ItemAncientDocument);
                    break;

                case "ItemArmor":
                    type = typeof(ItemArmor);
                    break;

                case "ItemArmorLastBoss":
                    type = typeof(ItemArmorLastBoss);
                    break;

                case "ItemArmorSdodr":
                    type = typeof(ItemArmorSdodr);
                    break;

                case "ItemCanInfinitySpecial_SuperHook":
                    type = typeof(ItemCanInfinitySpecial_SuperHook);
                    break;

                case "ItemCanSpecial_Blower":
                    type = typeof(ItemCanSpecial_Blower);
                    break;

                case "ItemCanSpecial_Chariot":
                    type = typeof(ItemCanSpecial_Chariot);
                    break;

                case "ItemCanSpecial_InkStorm":
                    type = typeof(ItemCanSpecial_InkStorm);
                    break;

                case "ItemCanSpecial_Jetpack":
                    type = typeof(ItemCanSpecial_Jetpack);
                    break;

                case "ItemCanSpecial_MicroLaser":
                    type = typeof(ItemCanSpecial_MicroLaser);
                    break;

                case "ItemCanSpecial_MultiMissile":
                    type = typeof(ItemCanSpecial_MultiMissile);
                    break;

                case "ItemCanSpecial_ShockSonar":
                    type = typeof(ItemCanSpecial_ShockSonar);
                    break;

                case "ItemCanSpecial_Skewer":
                    type = typeof(ItemCanSpecial_Skewer);
                    break;

                case "ItemCanSpecial_SuperHook":
                    type = typeof(ItemCanSpecial_SuperHook);
                    break;

                case "ItemCanSpecial_SuperLanding":
                    type = typeof(ItemCanSpecial_SuperLanding);
                    break;

                case "ItemCanSpecial_TripleTornado":
                    type = typeof(ItemCanSpecial_TripleTornado);
                    break;

                case "ItemCanSpecial_UltraShot":
                    type = typeof(ItemCanSpecial_UltraShot);
                    break;

                case "ItemCanSpecial_UltraStamp":
                    type = typeof(ItemCanSpecial_UltraStamp);
                    break;

                case "ItemCardKey":
                    type = typeof(ItemCardKey);
                    break;

                case "ItemCardPack":
                    type = typeof(ItemCardPack);
                    break;

                case "ItemDroneBatterySdodr":
                    type = typeof(ItemDroneBatterySdodr);
                    break;

                case "ItemEnergyCore":
                    type = typeof(ItemEnergyCore);
                    break;

                case "ItemFoodTicket":
                    type = typeof(ItemFoodTicket);
                    break;

                case "ItemGoldenDisk":
                    type = typeof(ItemGoldenDisk);
                    break;

                case "ItemIkura":
                    type = typeof(ItemIkura);
                    break;

                case "ItemIkuraBottle":
                    type = typeof(ItemIkuraBottle);
                    break;

                case "ItemIkuraLarge":
                    type = typeof(ItemIkuraLarge);
                    break;

                case "ItemInkBottle":
                    type = typeof(ItemInkBottle);
                    break;

                case "ItemInkBottleSdodr":
                    type = typeof(ItemInkBottleSdodr);
                    break;

                case "ItemLockerTreasure":
                    type = typeof(ItemLockerTreasure);
                    break;

                case "ItemSpecialChargeSdodr":
                    type = typeof(ItemSpecialChargeSdodr);
                    break;

                case "ItemStunBombSdodr_A":
                    type = typeof(ItemStunBombSdodr_A);
                    break;

                case "ItemStunBombSdodr_B":
                    type = typeof(ItemStunBombSdodr_B);
                    break;

                case "ItemStunBombSdodr_C":
                    type = typeof(ItemStunBombSdodr_C);
                    break;

                case "ItemWeaponBuffDocument":
                    type = typeof(ItemWeaponBuffDocument);
                    break;

                case "JumpPanel":
                    type = typeof(JumpPanel);
                    break;

                case "JumpPanelSdodr":
                    type = typeof(JumpPanelSdodr);
                    break;

                case "JumpPanelSdodrFast":
                    type = typeof(JumpPanelSdodrFast);
                    break;

                case "Kbi_NoCoreParent":
                    type = typeof(Kbi_NoCoreParent);
                    break;

                case "KebaInkCore":
                    type = typeof(KebaInkCore);
                    break;

                case "KebaInkCore_Big":
                    type = typeof(KebaInkCore_Big);
                    break;

                case "KebaInkCore_Small":
                    type = typeof(KebaInkCore_Small);
                    break;

                case "KingKeySdodr":
                    type = typeof(KingKeySdodr);
                    break;

                case "KingLetterSdodr":
                    type = typeof(KingLetterSdodr);
                    break;

                case "Level2":
                    type = typeof(Level2);
                    break;

                case "Lft_AbstractBlitzCompatibles":
                    type = typeof(Lft_AbstractBlitzCompatibles);
                    break;

                case "Lft_AbstractBlitzCompatibleSpectacle":
                    type = typeof(Lft_AbstractBlitzCompatibleSpectacle);
                    break;

                case "Lft_AbstractDrawer":
                    type = typeof(Lft_AbstractDrawer);
                    break;

                case "Lft_AbstractRotateTogglePoint":
                    type = typeof(Lft_AbstractRotateTogglePoint);
                    break;

                case "Lft_AbstractTwoPoint":
                    type = typeof(Lft_AbstractTwoPoint);
                    break;

                case "Lft_AlternaR05Pillar":
                    type = typeof(Lft_AlternaR05Pillar);
                    break;

                case "Lft_AsariSlope10":
                    type = typeof(Lft_AsariSlope10);
                    break;

                case "Lft_AsariSlope15":
                    type = typeof(Lft_AsariSlope15);
                    break;

                case "Lft_BaseWoodBoxCommanderChair":
                    type = typeof(Lft_BaseWoodBoxCommanderChair);
                    break;

                case "Lft_Bridge":
                    type = typeof(Lft_Bridge);
                    break;

                case "Lft_Bridge_AbstractRotateToggle":
                    type = typeof(Lft_Bridge_AbstractRotateToggle);
                    break;

                case "Lft_CarouselBridgeMoveA":
                    type = typeof(Lft_CarouselBridgeMoveA);
                    break;

                case "Lft_CarouselBridgeMoveB":
                    type = typeof(Lft_CarouselBridgeMoveB);
                    break;

                case "Lft_CarouselBridgeStop":
                    type = typeof(Lft_CarouselBridgeStop);
                    break;

                case "Lft_CarouselCircleFloorMove":
                    type = typeof(Lft_CarouselCircleFloorMove);
                    break;

                case "Lft_CarouselCircleFloorStop":
                    type = typeof(Lft_CarouselCircleFloorStop);
                    break;

                case "Lft_CarouselCircleFloor_Hoko":
                    type = typeof(Lft_CarouselCircleFloor_Hoko);
                    break;

                case "Lft_CarouselProtectA":
                    type = typeof(Lft_CarouselProtectA);
                    break;

                case "Lft_CarouselProtectB":
                    type = typeof(Lft_CarouselProtectB);
                    break;

                case "Lft_CarouselProtect_AreaB":
                    type = typeof(Lft_CarouselProtect_AreaB);
                    break;

                case "Lft_CarouselProtect_YaguraA":
                    type = typeof(Lft_CarouselProtect_YaguraA);
                    break;

                case "Lft_CarouselSignboardA":
                    type = typeof(Lft_CarouselSignboardA);
                    break;

                case "Lft_CarouselSignboardAStop":
                    type = typeof(Lft_CarouselSignboardAStop);
                    break;

                case "Lft_CarouselSignboardB":
                    type = typeof(Lft_CarouselSignboardB);
                    break;

                case "Lft_CarouselSignboardBStop":
                    type = typeof(Lft_CarouselSignboardBStop);
                    break;

                case "Lft_CarouselWallLeft":
                    type = typeof(Lft_CarouselWallLeft);
                    break;

                case "Lft_CarouselWallRight":
                    type = typeof(Lft_CarouselWallRight);
                    break;

                case "Lft_CarouselWallSide":
                    type = typeof(Lft_CarouselWallSide);
                    break;

                case "Lft_CarouselWallYagura":
                    type = typeof(Lft_CarouselWallYagura);
                    break;

                case "Lft_CaviarCan":
                    type = typeof(Lft_CaviarCan);
                    break;

                case "Lft_CenterPillarNoPaint_NoFld":
                    type = typeof(Lft_CenterPillarNoPaint_NoFld);
                    break;

                case "Lft_CenterPillarYagura_NoFld":
                    type = typeof(Lft_CenterPillarYagura_NoFld);
                    break;

                case "Lft_CenterPillar_NoFld":
                    type = typeof(Lft_CenterPillar_NoFld);
                    break;

                case "Lft_DecaCubeN30x60x60_Octa":
                    type = typeof(Lft_DecaCubeN30x60x60_Octa);
                    break;

                case "Lft_DitchTree":
                    type = typeof(Lft_DitchTree);
                    break;

                case "Lft_FldBG_Elevator01":
                    type = typeof(Lft_FldBG_Elevator01);
                    break;

                case "Lft_FldBG_ElevatorFrame":
                    type = typeof(Lft_FldBG_ElevatorFrame);
                    break;

                case "Lft_FldBG_MainFieldUnderConcrete":
                    type = typeof(Lft_FldBG_MainFieldUnderConcrete);
                    break;

                case "Lft_FldBG_MainFieldUnderConcreteTcl":
                    type = typeof(Lft_FldBG_MainFieldUnderConcreteTcl);
                    break;

                case "Lft_FldBG_Obj01":
                    type = typeof(Lft_FldBG_Obj01);
                    break;

                case "Lft_FldBG_Obj02":
                    type = typeof(Lft_FldBG_Obj02);
                    break;

                case "Lft_FldBG_Obj03":
                    type = typeof(Lft_FldBG_Obj03);
                    break;

                case "Lft_FldBG_Obj04":
                    type = typeof(Lft_FldBG_Obj04);
                    break;

                case "Lft_FldBG_Obj05":
                    type = typeof(Lft_FldBG_Obj05);
                    break;

                case "Lft_FldBG_Obj06":
                    type = typeof(Lft_FldBG_Obj06);
                    break;

                case "Lft_FldBG_Obj07":
                    type = typeof(Lft_FldBG_Obj07);
                    break;

                case "Lft_FldBG_Obj08":
                    type = typeof(Lft_FldBG_Obj08);
                    break;

                case "Lft_FldBG_Obj09":
                    type = typeof(Lft_FldBG_Obj09);
                    break;

                case "Lft_FldBG_Obj10":
                    type = typeof(Lft_FldBG_Obj10);
                    break;

                case "Lft_FldBG_Obj11":
                    type = typeof(Lft_FldBG_Obj11);
                    break;

                case "Lft_FldBG_SdodrRivalKing10F":
                    type = typeof(Lft_FldBG_SdodrRivalKing10F);
                    break;

                case "Lft_FldBG_SdodrRivalKing20F":
                    type = typeof(Lft_FldBG_SdodrRivalKing20F);
                    break;

                case "Lft_FldBG_Section00EscalatorStop":
                    type = typeof(Lft_FldBG_Section00EscalatorStop);
                    break;

                case "Lft_FldBG_Section01EscalatorStop":
                    type = typeof(Lft_FldBG_Section01EscalatorStop);
                    break;

                case "Lft_FldObj_AlternaBoss02":
                    type = typeof(Lft_FldObj_AlternaBoss02);
                    break;

                case "Lft_FldObj_AlternaBoss03":
                    type = typeof(Lft_FldObj_AlternaBoss03);
                    break;

                case "Lft_FldObj_AlternaBoss04":
                    type = typeof(Lft_FldObj_AlternaBoss04);
                    break;

                case "Lft_FldObj_AlternaBoss04Move":
                    type = typeof(Lft_FldObj_AlternaBoss04Move);
                    break;

                case "Lft_FldObj_AlternaCube15x15x15":
                    type = typeof(Lft_FldObj_AlternaCube15x15x15);
                    break;

                case "Lft_FldObj_AlternaCube15x45x15":
                    type = typeof(Lft_FldObj_AlternaCube15x45x15);
                    break;

                case "Lft_FldObj_AlternaCube15x60x15":
                    type = typeof(Lft_FldObj_AlternaCube15x60x15);
                    break;

                case "Lft_FldObj_AlternaCube30x15x15":
                    type = typeof(Lft_FldObj_AlternaCube30x15x15);
                    break;

                case "Lft_FldObj_AlternaCube30x15x30":
                    type = typeof(Lft_FldObj_AlternaCube30x15x30);
                    break;

                case "Lft_FldObj_AlternaCube30x15x45":
                    type = typeof(Lft_FldObj_AlternaCube30x15x45);
                    break;

                case "Lft_FldObj_AlternaCube30x15x80":
                    type = typeof(Lft_FldObj_AlternaCube30x15x80);
                    break;

                case "Lft_FldObj_AlternaCube30x30x15":
                    type = typeof(Lft_FldObj_AlternaCube30x30x15);
                    break;

                case "Lft_FldObj_AlternaCube30x30x30":
                    type = typeof(Lft_FldObj_AlternaCube30x30x30);
                    break;

                case "Lft_FldObj_AlternaCube30x30x45":
                    type = typeof(Lft_FldObj_AlternaCube30x30x45);
                    break;

                case "Lft_FldObj_AlternaCube30x30x80":
                    type = typeof(Lft_FldObj_AlternaCube30x30x80);
                    break;

                case "Lft_FldObj_AlternaCube30x45x15":
                    type = typeof(Lft_FldObj_AlternaCube30x45x15);
                    break;

                case "Lft_FldObj_AlternaCube30x45x30":
                    type = typeof(Lft_FldObj_AlternaCube30x45x30);
                    break;

                case "Lft_FldObj_AlternaCube30x45x60":
                    type = typeof(Lft_FldObj_AlternaCube30x45x60);
                    break;

                case "Lft_FldObj_AlternaCube30x60x15":
                    type = typeof(Lft_FldObj_AlternaCube30x60x15);
                    break;

                case "Lft_FldObj_AlternaCube30x70x30":
                    type = typeof(Lft_FldObj_AlternaCube30x70x30);
                    break;

                case "Lft_FldObj_AlternaCube40x30x15":
                    type = typeof(Lft_FldObj_AlternaCube40x30x15);
                    break;

                case "Lft_FldObj_AlternaCube45x15x15":
                    type = typeof(Lft_FldObj_AlternaCube45x15x15);
                    break;

                case "Lft_FldObj_AlternaCube60x30x45":
                    type = typeof(Lft_FldObj_AlternaCube60x30x45);
                    break;

                case "Lft_FldObj_AlternaCube60x45x15":
                    type = typeof(Lft_FldObj_AlternaCube60x45x15);
                    break;

                case "Lft_FldObj_AlternaCube60x60x15":
                    type = typeof(Lft_FldObj_AlternaCube60x60x15);
                    break;

                case "Lft_FldObj_AlternaCube90x40x15":
                    type = typeof(Lft_FldObj_AlternaCube90x40x15);
                    break;

                case "Lft_FldObj_AlternaCubeNP15x15x15":
                    type = typeof(Lft_FldObj_AlternaCubeNP15x15x15);
                    break;

                case "Lft_FldObj_AlternaCubeNP15x30x15":
                    type = typeof(Lft_FldObj_AlternaCubeNP15x30x15);
                    break;

                case "Lft_FldObj_AlternaCubeNP15x45x30":
                    type = typeof(Lft_FldObj_AlternaCubeNP15x45x30);
                    break;

                case "Lft_FldObj_AlternaCubeNP30x15x15":
                    type = typeof(Lft_FldObj_AlternaCubeNP30x15x15);
                    break;

                case "Lft_FldObj_AlternaCubeNP30x30x15":
                    type = typeof(Lft_FldObj_AlternaCubeNP30x30x15);
                    break;

                case "Lft_FldObj_AlternaCubeNP30x30x30":
                    type = typeof(Lft_FldObj_AlternaCubeNP30x30x30);
                    break;

                case "Lft_FldObj_AlternaCubeNP45x30x30":
                    type = typeof(Lft_FldObj_AlternaCubeNP45x30x30);
                    break;

                case "Lft_FldObj_AlternaCubeNP60x60x15":
                    type = typeof(Lft_FldObj_AlternaCubeNP60x60x15);
                    break;

                case "Lft_FldObj_AlternaCubeNP70x15x15":
                    type = typeof(Lft_FldObj_AlternaCubeNP70x15x15);
                    break;

                case "Lft_FldObj_AlternaMirrorSuccess":
                    type = typeof(Lft_FldObj_AlternaMirrorSuccess);
                    break;

                case "Lft_FldObj_AlternaPoleSuccess":
                    type = typeof(Lft_FldObj_AlternaPoleSuccess);
                    break;

                case "Lft_FldObj_AlternaR04Floor":
                    type = typeof(Lft_FldObj_AlternaR04Floor);
                    break;

                case "Lft_FldObj_AlternaSlope30x25x45":
                    type = typeof(Lft_FldObj_AlternaSlope30x25x45);
                    break;

                case "Lft_FldObj_AlternaSlope30x30x60":
                    type = typeof(Lft_FldObj_AlternaSlope30x30x60);
                    break;

                case "Lft_FldObj_AlternaSlope30x40x75":
                    type = typeof(Lft_FldObj_AlternaSlope30x40x75);
                    break;

                case "Lft_FldObj_AlternaSlope50x30x60":
                    type = typeof(Lft_FldObj_AlternaSlope50x30x60);
                    break;

                case "Lft_FldObj_AlternaSlope60x25x45":
                    type = typeof(Lft_FldObj_AlternaSlope60x25x45);
                    break;

                case "Lft_FldObj_AlternaSlope60x30x60":
                    type = typeof(Lft_FldObj_AlternaSlope60x30x60);
                    break;

                case "Lft_FldObj_AlternaSlope60x50x90":
                    type = typeof(Lft_FldObj_AlternaSlope60x50x90);
                    break;

                case "Lft_FldObj_AlternaTree":
                    type = typeof(Lft_FldObj_AlternaTree);
                    break;

                case "Lft_FldObj_ArmFloorLeft":
                    type = typeof(Lft_FldObj_ArmFloorLeft);
                    break;

                case "Lft_FldObj_ArmFloorRight":
                    type = typeof(Lft_FldObj_ArmFloorRight);
                    break;

                case "Lft_FldObj_AutoWalk00LiftCenterA":
                    type = typeof(Lft_FldObj_AutoWalk00LiftCenterA);
                    break;

                case "Lft_FldObj_AutoWalk00LiftCenterB":
                    type = typeof(Lft_FldObj_AutoWalk00LiftCenterB);
                    break;

                case "Lft_FldObj_AutoWalk00LiftEndLeftA":
                    type = typeof(Lft_FldObj_AutoWalk00LiftEndLeftA);
                    break;

                case "Lft_FldObj_AutoWalk00LiftEndLeftB":
                    type = typeof(Lft_FldObj_AutoWalk00LiftEndLeftB);
                    break;

                case "Lft_FldObj_AutoWalk00LiftEndRightA":
                    type = typeof(Lft_FldObj_AutoWalk00LiftEndRightA);
                    break;

                case "Lft_FldObj_AutoWalk00LiftEndRightB":
                    type = typeof(Lft_FldObj_AutoWalk00LiftEndRightB);
                    break;

                case "Lft_FldObj_AutoWalk00LiftIntLeftA":
                    type = typeof(Lft_FldObj_AutoWalk00LiftIntLeftA);
                    break;

                case "Lft_FldObj_AutoWalk00LiftIntLeftB":
                    type = typeof(Lft_FldObj_AutoWalk00LiftIntLeftB);
                    break;

                case "Lft_FldObj_AutoWalk00LiftIntRightA":
                    type = typeof(Lft_FldObj_AutoWalk00LiftIntRightA);
                    break;

                case "Lft_FldObj_AutoWalk00LiftIntRightB":
                    type = typeof(Lft_FldObj_AutoWalk00LiftIntRightB);
                    break;

                case "Lft_FldObj_AutoWalk00LiftSide":
                    type = typeof(Lft_FldObj_AutoWalk00LiftSide);
                    break;

                case "Lft_FldObj_AutoWalk00LiftTowerA":
                    type = typeof(Lft_FldObj_AutoWalk00LiftTowerA);
                    break;

                case "Lft_FldObj_AutoWalk00LiftTowerB":
                    type = typeof(Lft_FldObj_AutoWalk00LiftTowerB);
                    break;

                case "Lft_FldObj_AutoWalk00_PackPnt":
                    type = typeof(Lft_FldObj_AutoWalk00_PackPnt);
                    break;

                case "Lft_FldObj_AutoWalk00_PackVar":
                    type = typeof(Lft_FldObj_AutoWalk00_PackVar);
                    break;

                case "Lft_FldObj_AutoWalk00_PackVcl":
                    type = typeof(Lft_FldObj_AutoWalk00_PackVcl);
                    break;

                case "Lft_FldObj_AutoWalk00_PackVgl":
                    type = typeof(Lft_FldObj_AutoWalk00_PackVgl);
                    break;

                case "Lft_FldObj_AutoWalk00_PackVlf":
                    type = typeof(Lft_FldObj_AutoWalk00_PackVlf);
                    break;

                case "Lft_FldObj_BankaraCityFestAnnounce":
                    type = typeof(Lft_FldObj_BankaraCityFestAnnounce);
                    break;

                case "Lft_FldObj_BankaraCityFestDayOrSunset":
                    type = typeof(Lft_FldObj_BankaraCityFestDayOrSunset);
                    break;

                case "Lft_FldObj_BankaraCityFestFinish":
                    type = typeof(Lft_FldObj_BankaraCityFestFinish);
                    break;

                case "Lft_FldObj_BankaraCityFestFirst":
                    type = typeof(Lft_FldObj_BankaraCityFestFirst);
                    break;

                case "Lft_FldObj_BankaraCityFestSecond":
                    type = typeof(Lft_FldObj_BankaraCityFestSecond);
                    break;

                case "Lft_FldObj_BigRunCarousel03_GeneralBox":
                    type = typeof(Lft_FldObj_BigRunCarousel03_GeneralBox);
                    break;

                case "Lft_FldObj_BigRunUpland03_GeneralBox":
                    type = typeof(Lft_FldObj_BigRunUpland03_GeneralBox);
                    break;

                case "Lft_FldObj_BigSlope00_CenterBoard":
                    type = typeof(Lft_FldObj_BigSlope00_CenterBoard);
                    break;

                case "Lft_FldObj_BigSlope00_CenterLeaf":
                    type = typeof(Lft_FldObj_BigSlope00_CenterLeaf);
                    break;

                case "Lft_FldObj_BigSlope00_CenterWall":
                    type = typeof(Lft_FldObj_BigSlope00_CenterWall);
                    break;

                case "Lft_FldObj_BigSlope00_Net":
                    type = typeof(Lft_FldObj_BigSlope00_Net);
                    break;

                case "Lft_FldObj_BigSlope00_PackPnt":
                    type = typeof(Lft_FldObj_BigSlope00_PackPnt);
                    break;

                case "Lft_FldObj_BigSlope00_PackTcl":
                    type = typeof(Lft_FldObj_BigSlope00_PackTcl);
                    break;

                case "Lft_FldObj_BigSlope00_PackVar":
                    type = typeof(Lft_FldObj_BigSlope00_PackVar);
                    break;

                case "Lft_FldObj_BigSlope00_PackVcl":
                    type = typeof(Lft_FldObj_BigSlope00_PackVcl);
                    break;

                case "Lft_FldObj_BigSlope00_PackVgl":
                    type = typeof(Lft_FldObj_BigSlope00_PackVgl);
                    break;

                case "Lft_FldObj_BigSlope00_PackVlf":
                    type = typeof(Lft_FldObj_BigSlope00_PackVlf);
                    break;

                case "Lft_FldObj_BigSlope00_SideAsari":
                    type = typeof(Lft_FldObj_BigSlope00_SideAsari);
                    break;

                case "Lft_FldObj_BigSlope00_Slope":
                    type = typeof(Lft_FldObj_BigSlope00_Slope);
                    break;

                case "Lft_FldObj_BigSlope00_SlopeFill":
                    type = typeof(Lft_FldObj_BigSlope00_SlopeFill);
                    break;

                case "Lft_FldObj_BigSlope00_SlopeMiddle":
                    type = typeof(Lft_FldObj_BigSlope00_SlopeMiddle);
                    break;

                case "Lft_FldObj_BigSlope00_Tcl":
                    type = typeof(Lft_FldObj_BigSlope00_Tcl);
                    break;

                case "Lft_FldObj_BigSlope00_TopFloor":
                    type = typeof(Lft_FldObj_BigSlope00_TopFloor);
                    break;

                case "Lft_FldObj_BigSlope00_TopSlope":
                    type = typeof(Lft_FldObj_BigSlope00_TopSlope);
                    break;

                case "Lft_FldObj_CarouselNoBR":
                    type = typeof(Lft_FldObj_CarouselNoBR);
                    break;

                case "Lft_FldObj_CarouselPillar00":
                    type = typeof(Lft_FldObj_CarouselPillar00);
                    break;

                case "Lft_FldObj_CarouselProtect_YaguraBWide":
                    type = typeof(Lft_FldObj_CarouselProtect_YaguraBWide);
                    break;

                case "Lft_FldObj_CarouselRespawn":
                    type = typeof(Lft_FldObj_CarouselRespawn);
                    break;

                case "Lft_FldObj_CarouselRespawnVgl":
                    type = typeof(Lft_FldObj_CarouselRespawnVgl);
                    break;

                case "Lft_FldObj_CarouselWallCenter":
                    type = typeof(Lft_FldObj_CarouselWallCenter);
                    break;

                case "Lft_FldObj_CarouselWallCenterVgl":
                    type = typeof(Lft_FldObj_CarouselWallCenterVgl);
                    break;

                case "Lft_FldObj_CraterParts02_LiftWall":
                    type = typeof(Lft_FldObj_CraterParts02_LiftWall);
                    break;

                case "Lft_FldObj_CraterParts03_BoxUp00":
                    type = typeof(Lft_FldObj_CraterParts03_BoxUp00);
                    break;

                case "Lft_FldObj_CraterParts03_BoxUp01":
                    type = typeof(Lft_FldObj_CraterParts03_BoxUp01);
                    break;

                case "Lft_FldObj_CraterParts03_BoxUp02":
                    type = typeof(Lft_FldObj_CraterParts03_BoxUp02);
                    break;

                case "Lft_FldObj_CraterParts03_BoxUp03":
                    type = typeof(Lft_FldObj_CraterParts03_BoxUp03);
                    break;

                case "Lft_FldObj_CraterParts03_BoxUp04":
                    type = typeof(Lft_FldObj_CraterParts03_BoxUp04);
                    break;

                case "Lft_FldObj_CraterParts03_WallDown":
                    type = typeof(Lft_FldObj_CraterParts03_WallDown);
                    break;

                case "Lft_FldObj_CreterParts00":
                    type = typeof(Lft_FldObj_CreterParts00);
                    break;

                case "Lft_FldObj_CreterParts01":
                    type = typeof(Lft_FldObj_CreterParts01);
                    break;

                case "Lft_FldObj_CreterParts02":
                    type = typeof(Lft_FldObj_CreterParts02);
                    break;

                case "Lft_FldObj_CreterParts03":
                    type = typeof(Lft_FldObj_CreterParts03);
                    break;

                case "Lft_FldObj_District":
                    type = typeof(Lft_FldObj_District);
                    break;

                case "Lft_FldObj_District00_PackPnt":
                    type = typeof(Lft_FldObj_District00_PackPnt);
                    break;

                case "Lft_FldObj_District00_PackTcl":
                    type = typeof(Lft_FldObj_District00_PackTcl);
                    break;

                case "Lft_FldObj_District00_PackVar":
                    type = typeof(Lft_FldObj_District00_PackVar);
                    break;

                case "Lft_FldObj_District00_PackVcl":
                    type = typeof(Lft_FldObj_District00_PackVcl);
                    break;

                case "Lft_FldObj_District00_PackVgl":
                    type = typeof(Lft_FldObj_District00_PackVgl);
                    break;

                case "Lft_FldObj_District00_PackVlf":
                    type = typeof(Lft_FldObj_District00_PackVlf);
                    break;

                case "Lft_FldObj_DistrictAnotherB":
                    type = typeof(Lft_FldObj_DistrictAnotherB);
                    break;

                case "Lft_FldObj_DistrictBorder":
                    type = typeof(Lft_FldObj_DistrictBorder);
                    break;

                case "Lft_FldObj_DistrictBoxVlf":
                    type = typeof(Lft_FldObj_DistrictBoxVlf);
                    break;

                case "Lft_FldObj_DistrictBridgeNoHandrail":
                    type = typeof(Lft_FldObj_DistrictBridgeNoHandrail);
                    break;

                case "Lft_FldObj_DistrictBridgePnt":
                    type = typeof(Lft_FldObj_DistrictBridgePnt);
                    break;

                case "Lft_FldObj_DistrictBridgeVgl":
                    type = typeof(Lft_FldObj_DistrictBridgeVgl);
                    break;

                case "Lft_FldObj_DistrictFence":
                    type = typeof(Lft_FldObj_DistrictFence);
                    break;

                case "Lft_FldObj_DistrictGround":
                    type = typeof(Lft_FldObj_DistrictGround);
                    break;

                case "Lft_FldObj_DistrictHandrail":
                    type = typeof(Lft_FldObj_DistrictHandrail);
                    break;

                case "Lft_FldObj_DistrictHouseSlope":
                    type = typeof(Lft_FldObj_DistrictHouseSlope);
                    break;

                case "Lft_FldObj_DistrictMedianstrip":
                    type = typeof(Lft_FldObj_DistrictMedianstrip);
                    break;

                case "Lft_FldObj_DistrictMedianstripSide":
                    type = typeof(Lft_FldObj_DistrictMedianstripSide);
                    break;

                case "Lft_FldObj_DistrictMoveSlope":
                    type = typeof(Lft_FldObj_DistrictMoveSlope);
                    break;

                case "Lft_FldObj_DistrictSignBoard":
                    type = typeof(Lft_FldObj_DistrictSignBoard);
                    break;

                case "Lft_FldObj_DistrictSlope":
                    type = typeof(Lft_FldObj_DistrictSlope);
                    break;

                case "Lft_FldObj_DistrictSlope1":
                    type = typeof(Lft_FldObj_DistrictSlope1);
                    break;

                case "Lft_FldObj_DistrictTcl":
                    type = typeof(Lft_FldObj_DistrictTcl);
                    break;

                case "Lft_FldObj_DistrictWall":
                    type = typeof(Lft_FldObj_DistrictWall);
                    break;

                case "Lft_FldObj_Factory00AsariFence":
                    type = typeof(Lft_FldObj_Factory00AsariFence);
                    break;

                case "Lft_FldObj_Factory00CircleBox":
                    type = typeof(Lft_FldObj_Factory00CircleBox);
                    break;

                case "Lft_FldObj_Factory00Crane":
                    type = typeof(Lft_FldObj_Factory00Crane);
                    break;

                case "Lft_FldObj_Factory00Fence":
                    type = typeof(Lft_FldObj_Factory00Fence);
                    break;

                case "Lft_FldObj_Factory00Generator":
                    type = typeof(Lft_FldObj_Factory00Generator);
                    break;

                case "Lft_FldObj_Factory00HokoFence":
                    type = typeof(Lft_FldObj_Factory00HokoFence);
                    break;

                case "Lft_FldObj_Factory00Ikiri":
                    type = typeof(Lft_FldObj_Factory00Ikiri);
                    break;

                case "Lft_FldObj_Factory00Jump":
                    type = typeof(Lft_FldObj_Factory00Jump);
                    break;

                case "Lft_FldObj_Factory00StoneStatue":
                    type = typeof(Lft_FldObj_Factory00StoneStatue);
                    break;

                case "Lft_FldObj_Factory00WingBoxAP":
                    type = typeof(Lft_FldObj_Factory00WingBoxAP);
                    break;

                case "Lft_FldObj_Factory00_PackPnt":
                    type = typeof(Lft_FldObj_Factory00_PackPnt);
                    break;

                case "Lft_FldObj_Factory00_PackVar":
                    type = typeof(Lft_FldObj_Factory00_PackVar);
                    break;

                case "Lft_FldObj_Factory00_PackVcl":
                    type = typeof(Lft_FldObj_Factory00_PackVcl);
                    break;

                case "Lft_FldObj_Factory00_PackVgl":
                    type = typeof(Lft_FldObj_Factory00_PackVgl);
                    break;

                case "Lft_FldObj_Factory00_PackVlf":
                    type = typeof(Lft_FldObj_Factory00_PackVlf);
                    break;

                case "Lft_FldObj_FakeCliff":
                    type = typeof(Lft_FldObj_FakeCliff);
                    break;

                case "Lft_FldObj_GeneralBoxNoPaintDither15x15x15":
                    type = typeof(Lft_FldObj_GeneralBoxNoPaintDither15x15x15);
                    break;

                case "Lft_FldObj_GeneralBoxNoPaintDither30x15x15":
                    type = typeof(Lft_FldObj_GeneralBoxNoPaintDither30x15x15);
                    break;

                case "Lft_FldObj_GeneralBoxNoPaint_45x15x15_Blitz":
                    type = typeof(Lft_FldObj_GeneralBoxNoPaint_45x15x15_Blitz);
                    break;

                case "Lft_FldObj_GeneralBox_Upland_Blitz":
                    type = typeof(Lft_FldObj_GeneralBox_Upland_Blitz);
                    break;

                case "Lft_FldObj_GeneralSlope_60x30x60_Pivot":
                    type = typeof(Lft_FldObj_GeneralSlope_60x30x60_Pivot);
                    break;

                case "Lft_FldObj_Hiagari03Area":
                    type = typeof(Lft_FldObj_Hiagari03Area);
                    break;

                case "Lft_FldObj_Hiagari03AreaFloat":
                    type = typeof(Lft_FldObj_Hiagari03AreaFloat);
                    break;

                case "Lft_FldObj_Hiagari03AreaPlatform":
                    type = typeof(Lft_FldObj_Hiagari03AreaPlatform);
                    break;

                case "Lft_FldObj_Hiagari03Asari":
                    type = typeof(Lft_FldObj_Hiagari03Asari);
                    break;

                case "Lft_FldObj_Hiagari03Hoko":
                    type = typeof(Lft_FldObj_Hiagari03Hoko);
                    break;

                case "Lft_FldObj_Hiagari03HokoFloat":
                    type = typeof(Lft_FldObj_Hiagari03HokoFloat);
                    break;

                case "Lft_FldObj_Hiagari03Platform":
                    type = typeof(Lft_FldObj_Hiagari03Platform);
                    break;

                case "Lft_FldObj_Hiagari03Pnt":
                    type = typeof(Lft_FldObj_Hiagari03Pnt);
                    break;

                case "Lft_FldObj_Hiagari03PntFloat":
                    type = typeof(Lft_FldObj_Hiagari03PntFloat);
                    break;

                case "Lft_FldObj_Hiagari03Square":
                    type = typeof(Lft_FldObj_Hiagari03Square);
                    break;

                case "Lft_FldObj_Hiagari03Yagura":
                    type = typeof(Lft_FldObj_Hiagari03Yagura);
                    break;

                case "Lft_FldObj_Hiagari03YaguraFloat":
                    type = typeof(Lft_FldObj_Hiagari03YaguraFloat);
                    break;

                case "Lft_FldObj_Hiagari03YaguraPlatform":
                    type = typeof(Lft_FldObj_Hiagari03YaguraPlatform);
                    break;

                case "Lft_FldObj_Hiagari03YaguraRoad":
                    type = typeof(Lft_FldObj_Hiagari03YaguraRoad);
                    break;

                case "Lft_FldObj_Hiagari04Area":
                    type = typeof(Lft_FldObj_Hiagari04Area);
                    break;

                case "Lft_FldObj_Hiagari04AreaFloat":
                    type = typeof(Lft_FldObj_Hiagari04AreaFloat);
                    break;

                case "Lft_FldObj_Hiagari04AreaPlatform":
                    type = typeof(Lft_FldObj_Hiagari04AreaPlatform);
                    break;

                case "Lft_FldObj_Hiagari04Asari":
                    type = typeof(Lft_FldObj_Hiagari04Asari);
                    break;

                case "Lft_FldObj_Hiagari04AsariFloat":
                    type = typeof(Lft_FldObj_Hiagari04AsariFloat);
                    break;

                case "Lft_FldObj_Hiagari04Block":
                    type = typeof(Lft_FldObj_Hiagari04Block);
                    break;

                case "Lft_FldObj_Hiagari04Hoko":
                    type = typeof(Lft_FldObj_Hiagari04Hoko);
                    break;

                case "Lft_FldObj_Hiagari04HokoFloat":
                    type = typeof(Lft_FldObj_Hiagari04HokoFloat);
                    break;

                case "Lft_FldObj_Hiagari04Platform":
                    type = typeof(Lft_FldObj_Hiagari04Platform);
                    break;

                case "Lft_FldObj_Hiagari04Pnt":
                    type = typeof(Lft_FldObj_Hiagari04Pnt);
                    break;

                case "Lft_FldObj_Hiagari04PntFloat":
                    type = typeof(Lft_FldObj_Hiagari04PntFloat);
                    break;

                case "Lft_FldObj_Hiagari04Square":
                    type = typeof(Lft_FldObj_Hiagari04Square);
                    break;

                case "Lft_FldObj_Hiagari04SquareFence":
                    type = typeof(Lft_FldObj_Hiagari04SquareFence);
                    break;

                case "Lft_FldObj_Hiagari04Tcl":
                    type = typeof(Lft_FldObj_Hiagari04Tcl);
                    break;

                case "Lft_FldObj_Hiagari04TclFloat":
                    type = typeof(Lft_FldObj_Hiagari04TclFloat);
                    break;

                case "Lft_FldObj_Hiagari04TclRoad":
                    type = typeof(Lft_FldObj_Hiagari04TclRoad);
                    break;

                case "Lft_FldObj_Hiagari04Yagura":
                    type = typeof(Lft_FldObj_Hiagari04Yagura);
                    break;

                case "Lft_FldObj_Hiagari04YaguraFloat":
                    type = typeof(Lft_FldObj_Hiagari04YaguraFloat);
                    break;

                case "Lft_FldObj_Hiagari04YaguraPlatform":
                    type = typeof(Lft_FldObj_Hiagari04YaguraPlatform);
                    break;

                case "Lft_FldObj_Hiagari04YaguraRoad":
                    type = typeof(Lft_FldObj_Hiagari04YaguraRoad);
                    break;

                case "Lft_FldObj_Hiagari04_NoTcl":
                    type = typeof(Lft_FldObj_Hiagari04_NoTcl);
                    break;

                case "Lft_FldObj_HiagariWater":
                    type = typeof(Lft_FldObj_HiagariWater);
                    break;

                case "Lft_FldObj_HirameBottom":
                    type = typeof(Lft_FldObj_HirameBottom);
                    break;

                case "Lft_FldObj_HirameBottomAsari":
                    type = typeof(Lft_FldObj_HirameBottomAsari);
                    break;

                case "Lft_FldObj_HiramePaint":
                    type = typeof(Lft_FldObj_HiramePaint);
                    break;

                case "Lft_FldObj_HirameSideArea":
                    type = typeof(Lft_FldObj_HirameSideArea);
                    break;

                case "Lft_FldObj_HirameSlopeArea":
                    type = typeof(Lft_FldObj_HirameSlopeArea);
                    break;

                case "Lft_FldObj_HirameTowerAP":
                    type = typeof(Lft_FldObj_HirameTowerAP);
                    break;

                case "Lft_FldObj_Joheki03_PackPnt":
                    type = typeof(Lft_FldObj_Joheki03_PackPnt);
                    break;

                case "Lft_FldObj_Joheki03_PackVar":
                    type = typeof(Lft_FldObj_Joheki03_PackVar);
                    break;

                case "Lft_FldObj_Joheki03_PackVcl":
                    type = typeof(Lft_FldObj_Joheki03_PackVcl);
                    break;

                case "Lft_FldObj_Joheki03_PackVgl":
                    type = typeof(Lft_FldObj_Joheki03_PackVgl);
                    break;

                case "Lft_FldObj_Joheki03_PackVlf":
                    type = typeof(Lft_FldObj_Joheki03_PackVlf);
                    break;

                case "Lft_FldObj_Jyoheki03RespawnArea":
                    type = typeof(Lft_FldObj_Jyoheki03RespawnArea);
                    break;

                case "Lft_FldObj_Jyoheki03RoofTop":
                    type = typeof(Lft_FldObj_Jyoheki03RoofTop);
                    break;

                case "Lft_FldObj_Jyoheki03SlopeHandrail":
                    type = typeof(Lft_FldObj_Jyoheki03SlopeHandrail);
                    break;

                case "Lft_FldObj_Jyoheki03Tcl":
                    type = typeof(Lft_FldObj_Jyoheki03Tcl);
                    break;

                case "Lft_FldObj_Jyoheki03VclStage":
                    type = typeof(Lft_FldObj_Jyoheki03VclStage);
                    break;

                case "Lft_FldObj_Jyoheki03VglStage":
                    type = typeof(Lft_FldObj_Jyoheki03VglStage);
                    break;

                case "Lft_FldObj_Jyoheki03VlfStage":
                    type = typeof(Lft_FldObj_Jyoheki03VlfStage);
                    break;

                case "Lft_FldObj_Jyoheki03_Water":
                    type = typeof(Lft_FldObj_Jyoheki03_Water);
                    break;

                case "Lft_FldObj_JyohekiPlayGround":
                    type = typeof(Lft_FldObj_JyohekiPlayGround);
                    break;

                case "Lft_FldObj_JyohekiPlayGroundNight":
                    type = typeof(Lft_FldObj_JyohekiPlayGroundNight);
                    break;

                case "Lft_FldObj_Kaisou04Pnt":
                    type = typeof(Lft_FldObj_Kaisou04Pnt);
                    break;

                case "Lft_FldObj_Kaisou04Var":
                    type = typeof(Lft_FldObj_Kaisou04Var);
                    break;

                case "Lft_FldObj_Kaisou04Vcl":
                    type = typeof(Lft_FldObj_Kaisou04Vcl);
                    break;

                case "Lft_FldObj_Kaisou04Vgl":
                    type = typeof(Lft_FldObj_Kaisou04Vgl);
                    break;

                case "Lft_FldObj_Kaisou04Vlf":
                    type = typeof(Lft_FldObj_Kaisou04Vlf);
                    break;

                case "Lft_FldObj_KaisouBox":
                    type = typeof(Lft_FldObj_KaisouBox);
                    break;

                case "Lft_FldObj_KaisouFence":
                    type = typeof(Lft_FldObj_KaisouFence);
                    break;

                case "Lft_FldObj_KaisouFillVgl":
                    type = typeof(Lft_FldObj_KaisouFillVgl);
                    break;

                case "Lft_FldObj_KaisouFloorVlf":
                    type = typeof(Lft_FldObj_KaisouFloorVlf);
                    break;

                case "Lft_FldObj_KaisouOwnBlock":
                    type = typeof(Lft_FldObj_KaisouOwnBlock);
                    break;

                case "Lft_FldObj_KaisouPillar":
                    type = typeof(Lft_FldObj_KaisouPillar);
                    break;

                case "Lft_FldObj_KaisouPoleTwo":
                    type = typeof(Lft_FldObj_KaisouPoleTwo);
                    break;

                case "Lft_FldObj_KaisouSideBlock":
                    type = typeof(Lft_FldObj_KaisouSideBlock);
                    break;

                case "Lft_FldObj_KaisouSlopeCenter":
                    type = typeof(Lft_FldObj_KaisouSlopeCenter);
                    break;

                case "Lft_FldObj_KaisouSlopeSide":
                    type = typeof(Lft_FldObj_KaisouSlopeSide);
                    break;

                case "Lft_FldObj_KaisouStage":
                    type = typeof(Lft_FldObj_KaisouStage);
                    break;

                case "Lft_FldObj_KaisouStageBox":
                    type = typeof(Lft_FldObj_KaisouStageBox);
                    break;

                case "Lft_FldObj_KaisouStepLeft":
                    type = typeof(Lft_FldObj_KaisouStepLeft);
                    break;

                case "Lft_FldObj_KaisouStepRight":
                    type = typeof(Lft_FldObj_KaisouStepRight);
                    break;

                case "Lft_FldObj_KaisouTcl":
                    type = typeof(Lft_FldObj_KaisouTcl);
                    break;

                case "Lft_FldObj_KaisouTower":
                    type = typeof(Lft_FldObj_KaisouTower);
                    break;

                case "Lft_FldObj_KaisouUsually":
                    type = typeof(Lft_FldObj_KaisouUsually);
                    break;

                case "Lft_FldObj_KaisouVclBox":
                    type = typeof(Lft_FldObj_KaisouVclBox);
                    break;

                case "Lft_FldObj_KaisouWallBlue":
                    type = typeof(Lft_FldObj_KaisouWallBlue);
                    break;

                case "Lft_FldObj_KaisouWallLarge":
                    type = typeof(Lft_FldObj_KaisouWallLarge);
                    break;

                case "Lft_FldObj_KaisouWallSmall":
                    type = typeof(Lft_FldObj_KaisouWallSmall);
                    break;

                case "Lft_FldObj_KumaRocketFloor":
                    type = typeof(Lft_FldObj_KumaRocketFloor);
                    break;

                case "Lft_FldObj_KumaRocketParts":
                    type = typeof(Lft_FldObj_KumaRocketParts);
                    break;

                case "Lft_FldObj_LineCenterBox":
                    type = typeof(Lft_FldObj_LineCenterBox);
                    break;

                case "Lft_FldObj_LineCenterField1":
                    type = typeof(Lft_FldObj_LineCenterField1);
                    break;

                case "Lft_FldObj_LineExceptTri":
                    type = typeof(Lft_FldObj_LineExceptTri);
                    break;

                case "Lft_FldObj_LineHokoRespawn":
                    type = typeof(Lft_FldObj_LineHokoRespawn);
                    break;

                case "Lft_FldObj_LineHokoWall":
                    type = typeof(Lft_FldObj_LineHokoWall);
                    break;

                case "Lft_FldObj_LineRespawnfield":
                    type = typeof(Lft_FldObj_LineRespawnfield);
                    break;

                case "Lft_FldObj_LineSideBigBox1":
                    type = typeof(Lft_FldObj_LineSideBigBox1);
                    break;

                case "Lft_FldObj_LineSideBox":
                    type = typeof(Lft_FldObj_LineSideBox);
                    break;

                case "Lft_FldObj_LineTriColor":
                    type = typeof(Lft_FldObj_LineTriColor);
                    break;

                case "Lft_FldObj_Line_Board":
                    type = typeof(Lft_FldObj_Line_Board);
                    break;

                case "Lft_FldObj_MsnBossTreasurePedestal":
                    type = typeof(Lft_FldObj_MsnBossTreasurePedestal);
                    break;

                case "Lft_FldObj_MsnMoai":
                    type = typeof(Lft_FldObj_MsnMoai);
                    break;

                case "Lft_FldObj_MsnStartCreanRoom":
                    type = typeof(Lft_FldObj_MsnStartCreanRoom);
                    break;

                case "Lft_FldObj_Nagasaki03Area":
                    type = typeof(Lft_FldObj_Nagasaki03Area);
                    break;

                case "Lft_FldObj_Nagasaki03BridgeSide":
                    type = typeof(Lft_FldObj_Nagasaki03BridgeSide);
                    break;

                case "Lft_FldObj_Nagasaki03Hoko":
                    type = typeof(Lft_FldObj_Nagasaki03Hoko);
                    break;

                case "Lft_FldObj_Nagasaki03NoBR":
                    type = typeof(Lft_FldObj_Nagasaki03NoBR);
                    break;

                case "Lft_FldObj_Nagasaki03NoHoko":
                    type = typeof(Lft_FldObj_Nagasaki03NoHoko);
                    break;

                case "Lft_FldObj_Nagasaki03NoTcl":
                    type = typeof(Lft_FldObj_Nagasaki03NoTcl);
                    break;

                case "Lft_FldObj_Nagasaki03Tcl":
                    type = typeof(Lft_FldObj_Nagasaki03Tcl);
                    break;

                case "Lft_FldObj_Nagasaki03Yagura":
                    type = typeof(Lft_FldObj_Nagasaki03Yagura);
                    break;

                case "Lft_FldObj_Pillar00_PackPnt":
                    type = typeof(Lft_FldObj_Pillar00_PackPnt);
                    break;

                case "Lft_FldObj_Pillar00_PackTcl":
                    type = typeof(Lft_FldObj_Pillar00_PackTcl);
                    break;

                case "Lft_FldObj_Pillar00_PackVar":
                    type = typeof(Lft_FldObj_Pillar00_PackVar);
                    break;

                case "Lft_FldObj_Pillar00_PackVcl":
                    type = typeof(Lft_FldObj_Pillar00_PackVcl);
                    break;

                case "Lft_FldObj_Pillar00_PackVgl":
                    type = typeof(Lft_FldObj_Pillar00_PackVgl);
                    break;

                case "Lft_FldObj_Pillar00_PackVlf":
                    type = typeof(Lft_FldObj_Pillar00_PackVlf);
                    break;

                case "Lft_FldObj_Pillar_Fence":
                    type = typeof(Lft_FldObj_Pillar_Fence);
                    break;

                case "Lft_FldObj_Pillar_HokoRightBox":
                    type = typeof(Lft_FldObj_Pillar_HokoRightBox);
                    break;

                case "Lft_FldObj_Pillar_IsolatedGround":
                    type = typeof(Lft_FldObj_Pillar_IsolatedGround);
                    break;

                case "Lft_FldObj_Pillar_JumpBox":
                    type = typeof(Lft_FldObj_Pillar_JumpBox);
                    break;

                case "Lft_FldObj_Pillar_JumpBoxHoko":
                    type = typeof(Lft_FldObj_Pillar_JumpBoxHoko);
                    break;

                case "Lft_FldObj_Pillar_LaneBox":
                    type = typeof(Lft_FldObj_Pillar_LaneBox);
                    break;

                case "Lft_FldObj_Pillar_LeftField":
                    type = typeof(Lft_FldObj_Pillar_LeftField);
                    break;

                case "Lft_FldObj_Pillar_Tcl":
                    type = typeof(Lft_FldObj_Pillar_Tcl);
                    break;

                case "Lft_FldObj_Pillar_Wall":
                    type = typeof(Lft_FldObj_Pillar_Wall);
                    break;

                case "Lft_FldObj_PipelineBase00":
                    type = typeof(Lft_FldObj_PipelineBase00);
                    break;

                case "Lft_FldObj_PipelineBase01":
                    type = typeof(Lft_FldObj_PipelineBase01);
                    break;

                case "Lft_FldObj_PipelineBase02":
                    type = typeof(Lft_FldObj_PipelineBase02);
                    break;

                case "Lft_FldObj_PipelineBase03":
                    type = typeof(Lft_FldObj_PipelineBase03);
                    break;

                case "Lft_FldObj_PipelineBase04":
                    type = typeof(Lft_FldObj_PipelineBase04);
                    break;

                case "Lft_FldObj_PipelineBase05":
                    type = typeof(Lft_FldObj_PipelineBase05);
                    break;

                case "Lft_FldObj_PipelineBase06":
                    type = typeof(Lft_FldObj_PipelineBase06);
                    break;

                case "Lft_FldObj_PipelineBase07":
                    type = typeof(Lft_FldObj_PipelineBase07);
                    break;

                case "Lft_FldObj_PipelineBase08":
                    type = typeof(Lft_FldObj_PipelineBase08);
                    break;

                case "Lft_FldObj_PipelineBase09":
                    type = typeof(Lft_FldObj_PipelineBase09);
                    break;

                case "Lft_FldObj_PipelineBase10":
                    type = typeof(Lft_FldObj_PipelineBase10);
                    break;

                case "Lft_FldObj_PipelineBase11":
                    type = typeof(Lft_FldObj_PipelineBase11);
                    break;

                case "Lft_FldObj_Pivot0300":
                    type = typeof(Lft_FldObj_Pivot0300);
                    break;

                case "Lft_FldObj_Pivot03_PackPnt":
                    type = typeof(Lft_FldObj_Pivot03_PackPnt);
                    break;

                case "Lft_FldObj_Pivot03_PackVar":
                    type = typeof(Lft_FldObj_Pivot03_PackVar);
                    break;

                case "Lft_FldObj_Pivot03_PackVcl":
                    type = typeof(Lft_FldObj_Pivot03_PackVcl);
                    break;

                case "Lft_FldObj_Pivot03_PackVgl":
                    type = typeof(Lft_FldObj_Pivot03_PackVgl);
                    break;

                case "Lft_FldObj_Pivot03_PackVlf":
                    type = typeof(Lft_FldObj_Pivot03_PackVlf);
                    break;

                case "Lft_FldObj_PivotAreaCenterDoor":
                    type = typeof(Lft_FldObj_PivotAreaCenterDoor);
                    break;

                case "Lft_FldObj_PivotAsariGoal":
                    type = typeof(Lft_FldObj_PivotAsariGoal);
                    break;

                case "Lft_FldObj_PivotDoorPnt":
                    type = typeof(Lft_FldObj_PivotDoorPnt);
                    break;

                case "Lft_FldObj_PivotHokoCenterDoor":
                    type = typeof(Lft_FldObj_PivotHokoCenterDoor);
                    break;

                case "Lft_FldObj_PivotYagura":
                    type = typeof(Lft_FldObj_PivotYagura);
                    break;

                case "Lft_FldObj_PivotYaguraCenterDoor":
                    type = typeof(Lft_FldObj_PivotYaguraCenterDoor);
                    break;

                case "Lft_FldObj_PlazaDecorationDay":
                    type = typeof(Lft_FldObj_PlazaDecorationDay);
                    break;

                case "Lft_FldObj_Propeller00_Pnt":
                    type = typeof(Lft_FldObj_Propeller00_Pnt);
                    break;

                case "Lft_FldObj_Propeller00_Var":
                    type = typeof(Lft_FldObj_Propeller00_Var);
                    break;

                case "Lft_FldObj_Propeller00_Vcl":
                    type = typeof(Lft_FldObj_Propeller00_Vcl);
                    break;

                case "Lft_FldObj_Propeller00_Vgl":
                    type = typeof(Lft_FldObj_Propeller00_Vgl);
                    break;

                case "Lft_FldObj_Propeller00_Vlf":
                    type = typeof(Lft_FldObj_Propeller00_Vlf);
                    break;

                case "Lft_FldObj_Propeller_Lift":
                    type = typeof(Lft_FldObj_Propeller_Lift);
                    break;

                case "Lft_FldObj_Propeller_Lift01":
                    type = typeof(Lft_FldObj_Propeller_Lift01);
                    break;

                case "Lft_FldObj_Propeller_Lift02":
                    type = typeof(Lft_FldObj_Propeller_Lift02);
                    break;

                case "Lft_FldObj_Propeller_Lift03":
                    type = typeof(Lft_FldObj_Propeller_Lift03);
                    break;

                case "Lft_FldObj_Propeller_Lift04":
                    type = typeof(Lft_FldObj_Propeller_Lift04);
                    break;

                case "Lft_FldObj_Propeller_Lift05":
                    type = typeof(Lft_FldObj_Propeller_Lift05);
                    break;

                case "Lft_FldObj_Propeller_Lift06":
                    type = typeof(Lft_FldObj_Propeller_Lift06);
                    break;

                case "Lft_FldObj_RocketBigLid00":
                    type = typeof(Lft_FldObj_RocketBigLid00);
                    break;

                case "Lft_FldObj_RocketFence60x30":
                    type = typeof(Lft_FldObj_RocketFence60x30);
                    break;

                case "Lft_FldObj_RocketFence60x60":
                    type = typeof(Lft_FldObj_RocketFence60x60);
                    break;

                case "Lft_FldObj_RocketGateDoor00":
                    type = typeof(Lft_FldObj_RocketGateDoor00);
                    break;

                case "Lft_FldObj_RocketGlassDoor00":
                    type = typeof(Lft_FldObj_RocketGlassDoor00);
                    break;

                case "Lft_FldObj_RocketGlassDoor01":
                    type = typeof(Lft_FldObj_RocketGlassDoor01);
                    break;

                case "Lft_FldObj_RocketLift00":
                    type = typeof(Lft_FldObj_RocketLift00);
                    break;

                case "Lft_FldObj_RocketLift01":
                    type = typeof(Lft_FldObj_RocketLift01);
                    break;

                case "Lft_FldObj_RocketLiftRack00":
                    type = typeof(Lft_FldObj_RocketLiftRack00);
                    break;

                case "Lft_FldObj_RocketLiftStage00":
                    type = typeof(Lft_FldObj_RocketLiftStage00);
                    break;

                case "Lft_FldObj_RocketLiftStage01":
                    type = typeof(Lft_FldObj_RocketLiftStage01);
                    break;

                case "Lft_FldObj_RocketLiftStage02":
                    type = typeof(Lft_FldObj_RocketLiftStage02);
                    break;

                case "Lft_FldObj_RocketNamazuTank":
                    type = typeof(Lft_FldObj_RocketNamazuTank);
                    break;

                case "Lft_FldObj_RocketRotateBridge00":
                    type = typeof(Lft_FldObj_RocketRotateBridge00);
                    break;

                case "Lft_FldObj_RocketRotateBridge01":
                    type = typeof(Lft_FldObj_RocketRotateBridge01);
                    break;

                case "Lft_FldObj_RocketRotateDoor00":
                    type = typeof(Lft_FldObj_RocketRotateDoor00);
                    break;

                case "Lft_FldObj_RocketRotateDoor01":
                    type = typeof(Lft_FldObj_RocketRotateDoor01);
                    break;

                case "Lft_FldObj_RocketRotateDoor02":
                    type = typeof(Lft_FldObj_RocketRotateDoor02);
                    break;

                case "Lft_FldObj_RocketRotateDoor03":
                    type = typeof(Lft_FldObj_RocketRotateDoor03);
                    break;

                case "Lft_FldObj_RocketRotateDoor04":
                    type = typeof(Lft_FldObj_RocketRotateDoor04);
                    break;

                case "Lft_FldObj_RocketShutterGate00":
                    type = typeof(Lft_FldObj_RocketShutterGate00);
                    break;

                case "Lft_FldObj_RocketSlideWall00":
                    type = typeof(Lft_FldObj_RocketSlideWall00);
                    break;

                case "Lft_FldObj_RocketVinylFloor30x60":
                    type = typeof(Lft_FldObj_RocketVinylFloor30x60);
                    break;

                case "Lft_FldObj_RocketVinylFloor60x60":
                    type = typeof(Lft_FldObj_RocketVinylFloor60x60);
                    break;

                case "Lft_FldObj_RocketVinylFloor60x60Box":
                    type = typeof(Lft_FldObj_RocketVinylFloor60x60Box);
                    break;

                case "Lft_FldObj_RocketVinylFloor90x60":
                    type = typeof(Lft_FldObj_RocketVinylFloor90x60);
                    break;

                case "Lft_FldObj_RubberPole00":
                    type = typeof(Lft_FldObj_RubberPole00);
                    break;

                case "Lft_FldObj_Ruins03Block":
                    type = typeof(Lft_FldObj_Ruins03Block);
                    break;

                case "Lft_FldObj_Ruins03BlockSide":
                    type = typeof(Lft_FldObj_Ruins03BlockSide);
                    break;

                case "Lft_FldObj_Ruins03Elevator":
                    type = typeof(Lft_FldObj_Ruins03Elevator);
                    break;

                case "Lft_FldObj_Ruins03GeneralBoxPnt":
                    type = typeof(Lft_FldObj_Ruins03GeneralBoxPnt);
                    break;

                case "Lft_FldObj_Ruins03GeneralBoxTcl":
                    type = typeof(Lft_FldObj_Ruins03GeneralBoxTcl);
                    break;

                case "Lft_FldObj_Ruins03GeneralBoxVar":
                    type = typeof(Lft_FldObj_Ruins03GeneralBoxVar);
                    break;

                case "Lft_FldObj_Ruins03GeneralBoxVcl":
                    type = typeof(Lft_FldObj_Ruins03GeneralBoxVcl);
                    break;

                case "Lft_FldObj_Ruins03GeneralBoxVgl":
                    type = typeof(Lft_FldObj_Ruins03GeneralBoxVgl);
                    break;

                case "Lft_FldObj_Ruins03GeneralBoxVlf":
                    type = typeof(Lft_FldObj_Ruins03GeneralBoxVlf);
                    break;

                case "Lft_FldObj_Ruins03PntStage":
                    type = typeof(Lft_FldObj_Ruins03PntStage);
                    break;

                case "Lft_FldObj_Ruins03SmallBlock":
                    type = typeof(Lft_FldObj_Ruins03SmallBlock);
                    break;

                case "Lft_FldObj_Ruins03TclStage":
                    type = typeof(Lft_FldObj_Ruins03TclStage);
                    break;

                case "Lft_FldObj_Ruins03VarCenter":
                    type = typeof(Lft_FldObj_Ruins03VarCenter);
                    break;

                case "Lft_FldObj_Ruins03VarStage":
                    type = typeof(Lft_FldObj_Ruins03VarStage);
                    break;

                case "Lft_FldObj_Ruins03VclStage":
                    type = typeof(Lft_FldObj_Ruins03VclStage);
                    break;

                case "Lft_FldObj_Ruins03VglStage":
                    type = typeof(Lft_FldObj_Ruins03VglStage);
                    break;

                case "Lft_FldObj_Ruins03VlfStage":
                    type = typeof(Lft_FldObj_Ruins03VlfStage);
                    break;

                case "Lft_FldObj_Scrap00_Block00":
                    type = typeof(Lft_FldObj_Scrap00_Block00);
                    break;

                case "Lft_FldObj_Scrap00_Block01":
                    type = typeof(Lft_FldObj_Scrap00_Block01);
                    break;

                case "Lft_FldObj_Scrap00_Block02":
                    type = typeof(Lft_FldObj_Scrap00_Block02);
                    break;

                case "Lft_FldObj_Scrap00_Box00":
                    type = typeof(Lft_FldObj_Scrap00_Box00);
                    break;

                case "Lft_FldObj_Scrap00_BrokenCar":
                    type = typeof(Lft_FldObj_Scrap00_BrokenCar);
                    break;

                case "Lft_FldObj_Scrap00_CenterNet00":
                    type = typeof(Lft_FldObj_Scrap00_CenterNet00);
                    break;

                case "Lft_FldObj_Scrap00_Net":
                    type = typeof(Lft_FldObj_Scrap00_Net);
                    break;

                case "Lft_FldObj_Scrap00_Plane00":
                    type = typeof(Lft_FldObj_Scrap00_Plane00);
                    break;

                case "Lft_FldObj_Scrap00_ScrapMountain00":
                    type = typeof(Lft_FldObj_Scrap00_ScrapMountain00);
                    break;

                case "Lft_FldObj_Scrap00_SideWall00":
                    type = typeof(Lft_FldObj_Scrap00_SideWall00);
                    break;

                case "Lft_FldObj_Scrap00_Slope":
                    type = typeof(Lft_FldObj_Scrap00_Slope);
                    break;

                case "Lft_FldObj_Scrap00_Slope01":
                    type = typeof(Lft_FldObj_Scrap00_Slope01);
                    break;

                case "Lft_FldObj_Scrap00_Step":
                    type = typeof(Lft_FldObj_Scrap00_Step);
                    break;

                case "Lft_FldObj_Scrap00_TclStage00":
                    type = typeof(Lft_FldObj_Scrap00_TclStage00);
                    break;

                case "Lft_FldObj_Scrap01BoxPntVar":
                    type = typeof(Lft_FldObj_Scrap01BoxPntVar);
                    break;

                case "Lft_FldObj_Scrap01BoxVgl":
                    type = typeof(Lft_FldObj_Scrap01BoxVgl);
                    break;

                case "Lft_FldObj_Scrap01Vcl":
                    type = typeof(Lft_FldObj_Scrap01Vcl);
                    break;

                case "Lft_FldObj_Scrap01Vlf":
                    type = typeof(Lft_FldObj_Scrap01Vlf);
                    break;

                case "Lft_FldObj_Scrap01_Bike":
                    type = typeof(Lft_FldObj_Scrap01_Bike);
                    break;

                case "Lft_FldObj_Scrap01_Cape":
                    type = typeof(Lft_FldObj_Scrap01_Cape);
                    break;

                case "Lft_FldObj_Scrap01_CapeVgl":
                    type = typeof(Lft_FldObj_Scrap01_CapeVgl);
                    break;

                case "Lft_FldObj_Scrap01_Center":
                    type = typeof(Lft_FldObj_Scrap01_Center);
                    break;

                case "Lft_FldObj_Scrap01_Fence":
                    type = typeof(Lft_FldObj_Scrap01_Fence);
                    break;

                case "Lft_FldObj_Scrap01_FenceVgl":
                    type = typeof(Lft_FldObj_Scrap01_FenceVgl);
                    break;

                case "Lft_FldObj_Scrap01_Slope":
                    type = typeof(Lft_FldObj_Scrap01_Slope);
                    break;

                case "Lft_FldObj_Scrap01_Stage":
                    type = typeof(Lft_FldObj_Scrap01_Stage);
                    break;

                case "Lft_FldObj_Scrap01_Step":
                    type = typeof(Lft_FldObj_Scrap01_Step);
                    break;

                case "Lft_FldObj_Scrap01_Wall":
                    type = typeof(Lft_FldObj_Scrap01_Wall);
                    break;

                case "Lft_FldObj_SdodrClearCornerSlopeOut60x30x60NP":
                    type = typeof(Lft_FldObj_SdodrClearCornerSlopeOut60x30x60NP);
                    break;

                case "Lft_FldObj_SdodrClearCube15x15x15NP":
                    type = typeof(Lft_FldObj_SdodrClearCube15x15x15NP);
                    break;

                case "Lft_FldObj_SdodrClearCube30x15x15NP":
                    type = typeof(Lft_FldObj_SdodrClearCube30x15x15NP);
                    break;

                case "Lft_FldObj_SdodrClearCube30x15x30NP":
                    type = typeof(Lft_FldObj_SdodrClearCube30x15x30NP);
                    break;

                case "Lft_FldObj_SdodrClearCube30x30x30NP":
                    type = typeof(Lft_FldObj_SdodrClearCube30x30x30NP);
                    break;

                case "Lft_FldObj_SdodrClearCube45x15x60NP":
                    type = typeof(Lft_FldObj_SdodrClearCube45x15x60NP);
                    break;

                case "Lft_FldObj_SdodrClearCube60x15x15NP":
                    type = typeof(Lft_FldObj_SdodrClearCube60x15x15NP);
                    break;

                case "Lft_FldObj_SdodrClearCube60x30x30NP":
                    type = typeof(Lft_FldObj_SdodrClearCube60x30x30NP);
                    break;

                case "Lft_FldObj_SdodrClearCube60x60x30NP":
                    type = typeof(Lft_FldObj_SdodrClearCube60x60x30NP);
                    break;

                case "Lft_FldObj_SdodrClearCube60x60x60NP":
                    type = typeof(Lft_FldObj_SdodrClearCube60x60x60NP);
                    break;

                case "Lft_FldObj_SdodrClearCube90x60x90NP":
                    type = typeof(Lft_FldObj_SdodrClearCube90x60x90NP);
                    break;

                case "Lft_FldObj_SdodrClearFloor120x15x30NP":
                    type = typeof(Lft_FldObj_SdodrClearFloor120x15x30NP);
                    break;

                case "Lft_FldObj_SdodrClearFloor120x15x60NP":
                    type = typeof(Lft_FldObj_SdodrClearFloor120x15x60NP);
                    break;

                case "Lft_FldObj_SdodrClearFloor120x15x90NP":
                    type = typeof(Lft_FldObj_SdodrClearFloor120x15x90NP);
                    break;

                case "Lft_FldObj_SdodrClearFloor60x15x60NP":
                    type = typeof(Lft_FldObj_SdodrClearFloor60x15x60NP);
                    break;

                case "Lft_FldObj_SdodrClearFloor90x15x30NP":
                    type = typeof(Lft_FldObj_SdodrClearFloor90x15x30NP);
                    break;

                case "Lft_FldObj_SdodrClearFloor90x15x60NP":
                    type = typeof(Lft_FldObj_SdodrClearFloor90x15x60NP);
                    break;

                case "Lft_FldObj_SdodrClearFloor90x15x90NP":
                    type = typeof(Lft_FldObj_SdodrClearFloor90x15x90NP);
                    break;

                case "Lft_FldObj_SdodrClearSlope30x15x30NP":
                    type = typeof(Lft_FldObj_SdodrClearSlope30x15x30NP);
                    break;

                case "Lft_FldObj_SdodrClearSlope60x30x60NP":
                    type = typeof(Lft_FldObj_SdodrClearSlope60x30x60NP);
                    break;

                case "Lft_FldObj_SdodrCornerSlopeIn30x15x30AP":
                    type = typeof(Lft_FldObj_SdodrCornerSlopeIn30x15x30AP);
                    break;

                case "Lft_FldObj_SdodrCornerSlopeIn60x15x60AP":
                    type = typeof(Lft_FldObj_SdodrCornerSlopeIn60x15x60AP);
                    break;

                case "Lft_FldObj_SdodrCornerSlopeIn60x30x60AP":
                    type = typeof(Lft_FldObj_SdodrCornerSlopeIn60x30x60AP);
                    break;

                case "Lft_FldObj_SdodrCornerSlopeOut30x15x30AP":
                    type = typeof(Lft_FldObj_SdodrCornerSlopeOut30x15x30AP);
                    break;

                case "Lft_FldObj_SdodrCornerSlopeOut60x15x60AP":
                    type = typeof(Lft_FldObj_SdodrCornerSlopeOut60x15x60AP);
                    break;

                case "Lft_FldObj_SdodrCornerSlopeOut60x30x60AP":
                    type = typeof(Lft_FldObj_SdodrCornerSlopeOut60x30x60AP);
                    break;

                case "Lft_FldObj_SdodrCurb120x5x5NP":
                    type = typeof(Lft_FldObj_SdodrCurb120x5x5NP);
                    break;

                case "Lft_FldObj_SdodrCurb15x5x5NP":
                    type = typeof(Lft_FldObj_SdodrCurb15x5x5NP);
                    break;

                case "Lft_FldObj_SdodrCurb180x5x5NP":
                    type = typeof(Lft_FldObj_SdodrCurb180x5x5NP);
                    break;

                case "Lft_FldObj_SdodrCurb30x5x5NP":
                    type = typeof(Lft_FldObj_SdodrCurb30x5x5NP);
                    break;

                case "Lft_FldObj_SdodrCurb45x5x5NP":
                    type = typeof(Lft_FldObj_SdodrCurb45x5x5NP);
                    break;

                case "Lft_FldObj_SdodrCurb60x5x5NP":
                    type = typeof(Lft_FldObj_SdodrCurb60x5x5NP);
                    break;

                case "Lft_FldObj_SdodrCurb90x5x5NP":
                    type = typeof(Lft_FldObj_SdodrCurb90x5x5NP);
                    break;

                case "Lft_FldObj_SdodrCurbCorner15x5x15NP":
                    type = typeof(Lft_FldObj_SdodrCurbCorner15x5x15NP);
                    break;

                case "Lft_FldObj_SdodrCurbCorner30x5x30NP":
                    type = typeof(Lft_FldObj_SdodrCurbCorner30x5x30NP);
                    break;

                case "Lft_FldObj_SdodrCurbCorner45x5x45NP":
                    type = typeof(Lft_FldObj_SdodrCurbCorner45x5x45NP);
                    break;

                case "Lft_FldObj_SdodrCurbCorner5x5x5NP":
                    type = typeof(Lft_FldObj_SdodrCurbCorner5x5x5NP);
                    break;

                case "Lft_Fldobj_SdodrCurbCornerHQfillNP":
                    type = typeof(Lft_Fldobj_SdodrCurbCornerHQfillNP);
                    break;

                case "Lft_FldObj_SdodrCurbforDelta30NP":
                    type = typeof(Lft_FldObj_SdodrCurbforDelta30NP);
                    break;

                case "Lft_FldObj_SdodrCurbforDelta45NP":
                    type = typeof(Lft_FldObj_SdodrCurbforDelta45NP);
                    break;

                case "Lft_FldObj_SdodrCurbforDelta5x5x5NP":
                    type = typeof(Lft_FldObj_SdodrCurbforDelta5x5x5NP);
                    break;

                case "Lft_FldObj_SdodrCurbforDelta60NP":
                    type = typeof(Lft_FldObj_SdodrCurbforDelta60NP);
                    break;

                case "Lft_FldObj_SdodrCurbforDelta90NP":
                    type = typeof(Lft_FldObj_SdodrCurbforDelta90NP);
                    break;

                case "Lft_FldObj_SdodrCurbSlope30x15x5NP":
                    type = typeof(Lft_FldObj_SdodrCurbSlope30x15x5NP);
                    break;

                case "Lft_FldObj_SdodrCurbSlope60x15x5NP":
                    type = typeof(Lft_FldObj_SdodrCurbSlope60x15x5NP);
                    break;

                case "Lft_FldObj_SdodrCurbSlope60x30x5NP":
                    type = typeof(Lft_FldObj_SdodrCurbSlope60x30x5NP);
                    break;

                case "Lft_FldObj_SdodrCurbSlope90x30x5NP":
                    type = typeof(Lft_FldObj_SdodrCurbSlope90x30x5NP);
                    break;

                case "Lft_FldObj_SdodrCurb_LiftObjBase_oneRoad":
                    type = typeof(Lft_FldObj_SdodrCurb_LiftObjBase_oneRoad);
                    break;

                case "Lft_FldObj_SdodrCurb_LiftObjBase_propellerRising":
                    type = typeof(Lft_FldObj_SdodrCurb_LiftObjBase_propellerRising);
                    break;

                case "Lft_FldObj_SdodrElevatorShaft":
                    type = typeof(Lft_FldObj_SdodrElevatorShaft);
                    break;

                case "Lft_FldObj_SdodrFenceEdge45x20x3FC":
                    type = typeof(Lft_FldObj_SdodrFenceEdge45x20x3FC);
                    break;

                case "Lft_FldObj_SdodrFenceEdge75x20x3FC":
                    type = typeof(Lft_FldObj_SdodrFenceEdge75x20x3FC);
                    break;

                case "Lft_FldObj_SdodrFenceFloor30x5x30FC":
                    type = typeof(Lft_FldObj_SdodrFenceFloor30x5x30FC);
                    break;

                case "Lft_FldObj_SdodrFenceFloor60x5x30FC":
                    type = typeof(Lft_FldObj_SdodrFenceFloor60x5x30FC);
                    break;

                case "Lft_FldObj_SdodrFenceFloor60x5x60FC":
                    type = typeof(Lft_FldObj_SdodrFenceFloor60x5x60FC);
                    break;

                case "Lft_FldObj_SdodrLiftObjBase_oneRoad":
                    type = typeof(Lft_FldObj_SdodrLiftObjBase_oneRoad);
                    break;

                case "Lft_FldObj_SdodrLiftObjBase_propellerRising":
                    type = typeof(Lft_FldObj_SdodrLiftObjBase_propellerRising);
                    break;

                case "Lft_FldObj_SdodrLiftObjBase_rising":
                    type = typeof(Lft_FldObj_SdodrLiftObjBase_rising);
                    break;

                case "Lft_FldObj_SdodrNeutralCube15x15x15AP":
                    type = typeof(Lft_FldObj_SdodrNeutralCube15x15x15AP);
                    break;

                case "Lft_FldObj_SdodrNeutralCube30x15x15AP":
                    type = typeof(Lft_FldObj_SdodrNeutralCube30x15x15AP);
                    break;

                case "Lft_FldObj_SdodrNeutralCube30x15x30AP":
                    type = typeof(Lft_FldObj_SdodrNeutralCube30x15x30AP);
                    break;

                case "Lft_FldObj_SdodrNeutralCube30x30x30AP":
                    type = typeof(Lft_FldObj_SdodrNeutralCube30x30x30AP);
                    break;

                case "Lft_FldObj_SdodrNeutralCube30x30x30TP":
                    type = typeof(Lft_FldObj_SdodrNeutralCube30x30x30TP);
                    break;

                case "Lft_FldObj_SdodrNeutralCube30x30x30XPA":
                    type = typeof(Lft_FldObj_SdodrNeutralCube30x30x30XPA);
                    break;

                case "Lft_FldObj_SdodrNeutralCube30x30x30XPB":
                    type = typeof(Lft_FldObj_SdodrNeutralCube30x30x30XPB);
                    break;

                case "Lft_FldObj_SdodrNeutralCube30x30x30XPC":
                    type = typeof(Lft_FldObj_SdodrNeutralCube30x30x30XPC);
                    break;

                case "Lft_FldObj_SdodrNeutralCube30x30x30XPD":
                    type = typeof(Lft_FldObj_SdodrNeutralCube30x30x30XPD);
                    break;

                case "Lft_FldObj_SdodrNeutralCube45x15x60AP":
                    type = typeof(Lft_FldObj_SdodrNeutralCube45x15x60AP);
                    break;

                case "Lft_FldObj_SdodrNeutralCube45x30x45TP":
                    type = typeof(Lft_FldObj_SdodrNeutralCube45x30x45TP);
                    break;

                case "Lft_FldObj_SdodrNeutralCube60x15x15AP":
                    type = typeof(Lft_FldObj_SdodrNeutralCube60x15x15AP);
                    break;

                case "Lft_FldObj_SdodrNeutralCube60x30x15AP":
                    type = typeof(Lft_FldObj_SdodrNeutralCube60x30x15AP);
                    break;

                case "Lft_FldObj_SdodrNeutralCube60x30x30AP":
                    type = typeof(Lft_FldObj_SdodrNeutralCube60x30x30AP);
                    break;

                case "Lft_FldObj_SdodrNeutralCube60x60x30AP":
                    type = typeof(Lft_FldObj_SdodrNeutralCube60x60x30AP);
                    break;

                case "Lft_FldObj_SdodrNeutralCube60x60x60AP":
                    type = typeof(Lft_FldObj_SdodrNeutralCube60x60x60AP);
                    break;

                case "Lft_FldObj_SdodrNeutralCube60x60x60TP":
                    type = typeof(Lft_FldObj_SdodrNeutralCube60x60x60TP);
                    break;

                case "Lft_FldObj_SdodrNeutralCube60x60x60XPA":
                    type = typeof(Lft_FldObj_SdodrNeutralCube60x60x60XPA);
                    break;

                case "Lft_FldObj_SdodrNeutralCube60x60x60XPB":
                    type = typeof(Lft_FldObj_SdodrNeutralCube60x60x60XPB);
                    break;

                case "Lft_FldObj_SdodrNeutralCube60x60x60XPC":
                    type = typeof(Lft_FldObj_SdodrNeutralCube60x60x60XPC);
                    break;

                case "Lft_FldObj_SdodrNeutralCube60x60x60XPD":
                    type = typeof(Lft_FldObj_SdodrNeutralCube60x60x60XPD);
                    break;

                case "Lft_FldObj_SdodrNeutralCube90x60x90AP":
                    type = typeof(Lft_FldObj_SdodrNeutralCube90x60x90AP);
                    break;

                case "Lft_FldObj_SdodrNeutralCube90x60x90TP":
                    type = typeof(Lft_FldObj_SdodrNeutralCube90x60x90TP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor120x15x120AP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor120x15x120AP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor120x15x120TP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor120x15x120TP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor120x15x30AP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor120x15x30AP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor120x15x30TP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor120x15x30TP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor120x15x60AP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor120x15x60AP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor120x15x60TP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor120x15x60TP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor120x15x90AP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor120x15x90AP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor120x15x90TP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor120x15x90TP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor180x15x180TP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor180x15x180TP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor180x15x60AP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor180x15x60AP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor180x15x60TP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor180x15x60TP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor180x15x90AP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor180x15x90AP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor180x15x90TP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor180x15x90TP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor300x15x300TP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor300x15x300TP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor60x15x60AP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor60x15x60AP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor60x15x60TP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor60x15x60TP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor90x15x30AP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor90x15x30AP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor90x15x30TP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor90x15x30TP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor90x15x45TP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor90x15x45TP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor90x15x60AP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor90x15x60AP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor90x15x60TP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor90x15x60TP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor90x15x90AP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor90x15x90AP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloor90x15x90TP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloor90x15x90TP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloorDelta30x15x30AP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloorDelta30x15x30AP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloorDelta30x15x30NP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloorDelta30x15x30NP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloorDelta30x15x30TP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloorDelta30x15x30TP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloorDelta45x15x45TP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloorDelta45x15x45TP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloorDelta60x15x60AP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloorDelta60x15x60AP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloorDelta60x15x60NP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloorDelta60x15x60NP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloorDelta60x15x60TP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloorDelta60x15x60TP);
                    break;

                case "Lft_FldObj_SdodrNeutralFloorDelta90x15x90TP":
                    type = typeof(Lft_FldObj_SdodrNeutralFloorDelta90x15x90TP);
                    break;

                case "Lft_FldObj_SdodrNeutralSlope30x15x30AP":
                    type = typeof(Lft_FldObj_SdodrNeutralSlope30x15x30AP);
                    break;

                case "Lft_FldObj_SdodrNeutralSlope30x15x30TP":
                    type = typeof(Lft_FldObj_SdodrNeutralSlope30x15x30TP);
                    break;

                case "Lft_FldObj_SdodrNeutralSlope30x15x60AP":
                    type = typeof(Lft_FldObj_SdodrNeutralSlope30x15x60AP);
                    break;

                case "Lft_FldObj_SdodrNeutralSlope30x15x60TP":
                    type = typeof(Lft_FldObj_SdodrNeutralSlope30x15x60TP);
                    break;

                case "Lft_FldObj_SdodrNeutralSlope30x30x60AP":
                    type = typeof(Lft_FldObj_SdodrNeutralSlope30x30x60AP);
                    break;

                case "Lft_FldObj_SdodrNeutralSlope30x30x60TP":
                    type = typeof(Lft_FldObj_SdodrNeutralSlope30x30x60TP);
                    break;

                case "Lft_FldObj_SdodrNeutralSlope60x15x60AP":
                    type = typeof(Lft_FldObj_SdodrNeutralSlope60x15x60AP);
                    break;

                case "Lft_FldObj_SdodrNeutralSlope60x15x60TP":
                    type = typeof(Lft_FldObj_SdodrNeutralSlope60x15x60TP);
                    break;

                case "Lft_FldObj_SdodrNeutralSlope60x30x60AP":
                    type = typeof(Lft_FldObj_SdodrNeutralSlope60x30x60AP);
                    break;

                case "Lft_FldObj_SdodrNeutralSlope60x30x60TP":
                    type = typeof(Lft_FldObj_SdodrNeutralSlope60x30x60TP);
                    break;

                case "Lft_FldObj_SdodrNeutralSlope90x15x60AP":
                    type = typeof(Lft_FldObj_SdodrNeutralSlope90x15x60AP);
                    break;

                case "Lft_FldObj_SdodrNeutralSlope90x15x60TP":
                    type = typeof(Lft_FldObj_SdodrNeutralSlope90x15x60TP);
                    break;

                case "Lft_FldObj_SdodrNeutralSlope90x30x60AP":
                    type = typeof(Lft_FldObj_SdodrNeutralSlope90x30x60AP);
                    break;

                case "Lft_FldObj_SdodrNeutralSlope90x30x60TP":
                    type = typeof(Lft_FldObj_SdodrNeutralSlope90x30x60TP);
                    break;

                case "Lft_FldObj_SdodrNeutralSlope90x30x90AP":
                    type = typeof(Lft_FldObj_SdodrNeutralSlope90x30x90AP);
                    break;

                case "Lft_FldObj_SdodrNeutralSlope90x30x90TP":
                    type = typeof(Lft_FldObj_SdodrNeutralSlope90x30x90TP);
                    break;

                case "Lft_FldObj_SdodrPlazaOrder2ndTower":
                    type = typeof(Lft_FldObj_SdodrPlazaOrder2ndTower);
                    break;

                case "Lft_FldObj_SdodrPlazaOrder2ndTowerBroken":
                    type = typeof(Lft_FldObj_SdodrPlazaOrder2ndTowerBroken);
                    break;

                case "Lft_FldObj_SdodrPlazaOrder2ndTowerTop":
                    type = typeof(Lft_FldObj_SdodrPlazaOrder2ndTowerTop);
                    break;

                case "Lft_FldObj_SdodrPlazaOrderExtraShop":
                    type = typeof(Lft_FldObj_SdodrPlazaOrderExtraShop);
                    break;

                case "Lft_FldObj_SdodrPlazaOrderFirstTower":
                    type = typeof(Lft_FldObj_SdodrPlazaOrderFirstTower);
                    break;

                case "Lft_FldObj_SdodrPlazaPartsDay":
                    type = typeof(Lft_FldObj_SdodrPlazaPartsDay);
                    break;

                case "Lft_FldObj_SdodrPlazaPartsFestAfter":
                    type = typeof(Lft_FldObj_SdodrPlazaPartsFestAfter);
                    break;

                case "Lft_FldObj_SdodrPlazaPartsFestBefore":
                    type = typeof(Lft_FldObj_SdodrPlazaPartsFestBefore);
                    break;

                case "Lft_FldObj_SdodrPlazaStagePillar":
                    type = typeof(Lft_FldObj_SdodrPlazaStagePillar);
                    break;

                case "Lft_FldObj_Sdodr_oneRoad_ball_Sdodr_oneRoad_ball_lift_0_cmn":
                    type = typeof(Lft_FldObj_Sdodr_oneRoad_ball_Sdodr_oneRoad_ball_lift_0_cmn);
                    break;

                case "Lft_FldObj_Sdodr_propellerRising_spawner_lift_0_cmn":
                    type = typeof(Lft_FldObj_Sdodr_propellerRising_spawner_lift_0_cmn);
                    break;

                case "Lft_FldObj_Sdodr_rising_area_Sdodr_rising_lift_0_cmn":
                    type = typeof(Lft_FldObj_Sdodr_rising_area_Sdodr_rising_lift_0_cmn);
                    break;

                case "Lft_FldObj_Sdodr_rising_ball_Sdodr_rising_lift_0_cmn":
                    type = typeof(Lft_FldObj_Sdodr_rising_ball_Sdodr_rising_lift_0_cmn);
                    break;

                case "Lft_FldObj_Sdodr_rising_chase_Sdodr_rising_lift_0_cmn":
                    type = typeof(Lft_FldObj_Sdodr_rising_chase_Sdodr_rising_lift_0_cmn);
                    break;

                case "Lft_FldObj_Sdodr_rising_lift_Sdodr_rising_lift_0_cmn":
                    type = typeof(Lft_FldObj_Sdodr_rising_lift_Sdodr_rising_lift_0_cmn);
                    break;

                case "Lft_FldObj_Sdodr_rising_spawner_2_Sdodr_rising_lift_0_cmn":
                    type = typeof(Lft_FldObj_Sdodr_rising_spawner_2_Sdodr_rising_lift_0_cmn);
                    break;

                case "Lft_FldObj_Sdodr_rising_spawner_Sdodr_rising_lift_0_cmn":
                    type = typeof(Lft_FldObj_Sdodr_rising_spawner_Sdodr_rising_lift_0_cmn);
                    break;

                case "Lft_FldObj_Section00_Block":
                    type = typeof(Lft_FldObj_Section00_Block);
                    break;

                case "Lft_FldObj_Section00_CenterBlock":
                    type = typeof(Lft_FldObj_Section00_CenterBlock);
                    break;

                case "Lft_FldObj_Section00_CenterSide":
                    type = typeof(Lft_FldObj_Section00_CenterSide);
                    break;

                case "Lft_FldObj_Section00_CenterSideTcl":
                    type = typeof(Lft_FldObj_Section00_CenterSideTcl);
                    break;

                case "Lft_FldObj_Section00_CubeCmn":
                    type = typeof(Lft_FldObj_Section00_CubeCmn);
                    break;

                case "Lft_FldObj_Section00_CubePnt":
                    type = typeof(Lft_FldObj_Section00_CubePnt);
                    break;

                case "Lft_FldObj_Section00_CubeTcl":
                    type = typeof(Lft_FldObj_Section00_CubeTcl);
                    break;

                case "Lft_FldObj_Section00_CubeVar":
                    type = typeof(Lft_FldObj_Section00_CubeVar);
                    break;

                case "Lft_FldObj_Section00_CubeVcl":
                    type = typeof(Lft_FldObj_Section00_CubeVcl);
                    break;

                case "Lft_FldObj_Section00_CubeVgl":
                    type = typeof(Lft_FldObj_Section00_CubeVgl);
                    break;

                case "Lft_FldObj_Section00_CubeVlf":
                    type = typeof(Lft_FldObj_Section00_CubeVlf);
                    break;

                case "Lft_FldObj_Section00_Fence":
                    type = typeof(Lft_FldObj_Section00_Fence);
                    break;

                case "Lft_FldObj_Section00_FloorFill":
                    type = typeof(Lft_FldObj_Section00_FloorFill);
                    break;

                case "Lft_FldObj_Section00_FloorFillVgl":
                    type = typeof(Lft_FldObj_Section00_FloorFillVgl);
                    break;

                case "Lft_FldObj_Section00_Respawn":
                    type = typeof(Lft_FldObj_Section00_Respawn);
                    break;

                case "Lft_FldObj_Section00_RespawnTcl":
                    type = typeof(Lft_FldObj_Section00_RespawnTcl);
                    break;

                case "Lft_FldObj_Section00_RightBlock":
                    type = typeof(Lft_FldObj_Section00_RightBlock);
                    break;

                case "Lft_FldObj_Section00_RightFloorLong":
                    type = typeof(Lft_FldObj_Section00_RightFloorLong);
                    break;

                case "Lft_FldObj_Section00_RightFloorShort":
                    type = typeof(Lft_FldObj_Section00_RightFloorShort);
                    break;

                case "Lft_FldObj_Section00_RightFloorSlope":
                    type = typeof(Lft_FldObj_Section00_RightFloorSlope);
                    break;

                case "Lft_FldObj_Section00_RightFloorTcl":
                    type = typeof(Lft_FldObj_Section00_RightFloorTcl);
                    break;

                case "Lft_FldObj_Section00_RightRoute":
                    type = typeof(Lft_FldObj_Section00_RightRoute);
                    break;

                case "Lft_FldObj_Section00_TowerHigh":
                    type = typeof(Lft_FldObj_Section00_TowerHigh);
                    break;

                case "Lft_FldObj_Section00_TowerLow":
                    type = typeof(Lft_FldObj_Section00_TowerLow);
                    break;

                case "Lft_FldObj_Section01_PntVarSet":
                    type = typeof(Lft_FldObj_Section01_PntVarSet);
                    break;

                case "Lft_FldObj_Section01_VclSet":
                    type = typeof(Lft_FldObj_Section01_VclSet);
                    break;

                case "Lft_FldObj_Section01_VglSet":
                    type = typeof(Lft_FldObj_Section01_VglSet);
                    break;

                case "Lft_FldObj_Section01_VlfSet":
                    type = typeof(Lft_FldObj_Section01_VlfSet);
                    break;

                case "Lft_FldObj_Spider00_PackPnt":
                    type = typeof(Lft_FldObj_Spider00_PackPnt);
                    break;

                case "Lft_FldObj_Spider00_PackTcl":
                    type = typeof(Lft_FldObj_Spider00_PackTcl);
                    break;

                case "Lft_FldObj_Spider00_PackVar":
                    type = typeof(Lft_FldObj_Spider00_PackVar);
                    break;

                case "Lft_FldObj_Spider00_PackVcl":
                    type = typeof(Lft_FldObj_Spider00_PackVcl);
                    break;

                case "Lft_FldObj_Spider00_PackVgl":
                    type = typeof(Lft_FldObj_Spider00_PackVgl);
                    break;

                case "Lft_FldObj_Spider00_PackVlf":
                    type = typeof(Lft_FldObj_Spider00_PackVlf);
                    break;

                case "Lft_FldObj_SurimiStuidoBG":
                    type = typeof(Lft_FldObj_SurimiStuidoBG);
                    break;

                case "Lft_FldObj_Temple00_Block01":
                    type = typeof(Lft_FldObj_Temple00_Block01);
                    break;

                case "Lft_FldObj_Temple00_Block02":
                    type = typeof(Lft_FldObj_Temple00_Block02);
                    break;

                case "Lft_FldObj_Temple00_Block03":
                    type = typeof(Lft_FldObj_Temple00_Block03);
                    break;

                case "Lft_FldObj_Temple00_Block04":
                    type = typeof(Lft_FldObj_Temple00_Block04);
                    break;

                case "Lft_FldObj_Temple00_BlockNP01":
                    type = typeof(Lft_FldObj_Temple00_BlockNP01);
                    break;

                case "Lft_FldObj_Temple00_Fence01":
                    type = typeof(Lft_FldObj_Temple00_Fence01);
                    break;

                case "Lft_FldObj_Temple00_Fence02":
                    type = typeof(Lft_FldObj_Temple00_Fence02);
                    break;

                case "Lft_FldObj_Temple00_GatiRule":
                    type = typeof(Lft_FldObj_Temple00_GatiRule);
                    break;

                case "Lft_FldObj_Temple00_Glass01":
                    type = typeof(Lft_FldObj_Temple00_Glass01);
                    break;

                case "Lft_FldObj_Temple00_NormalRule":
                    type = typeof(Lft_FldObj_Temple00_NormalRule);
                    break;

                case "Lft_FldObj_Temple00_PillarSide":
                    type = typeof(Lft_FldObj_Temple00_PillarSide);
                    break;

                case "Lft_FldObj_Temple00_RightStage01":
                    type = typeof(Lft_FldObj_Temple00_RightStage01);
                    break;

                case "Lft_FldObj_Temple00_RightStage02":
                    type = typeof(Lft_FldObj_Temple00_RightStage02);
                    break;

                case "Lft_FldObj_Temple00_Slope01":
                    type = typeof(Lft_FldObj_Temple00_Slope01);
                    break;

                case "Lft_FldObj_Temple00_SlopeNP01":
                    type = typeof(Lft_FldObj_Temple00_SlopeNP01);
                    break;

                case "Lft_FldObj_Temple00_SlopeNP02":
                    type = typeof(Lft_FldObj_Temple00_SlopeNP02);
                    break;

                case "Lft_FldObj_Temple00_StageEndPnt":
                    type = typeof(Lft_FldObj_Temple00_StageEndPnt);
                    break;

                case "Lft_FldObj_Temple00_Tcl":
                    type = typeof(Lft_FldObj_Temple00_Tcl);
                    break;

                case "Lft_FldObj_Temple01_PntSet":
                    type = typeof(Lft_FldObj_Temple01_PntSet);
                    break;

                case "Lft_FldObj_Temple01_VarSet":
                    type = typeof(Lft_FldObj_Temple01_VarSet);
                    break;

                case "Lft_FldObj_Temple01_VclSet":
                    type = typeof(Lft_FldObj_Temple01_VclSet);
                    break;

                case "Lft_FldObj_Temple01_VglSet":
                    type = typeof(Lft_FldObj_Temple01_VglSet);
                    break;

                case "Lft_FldObj_Temple01_VlfSet":
                    type = typeof(Lft_FldObj_Temple01_VlfSet);
                    break;

                case "Lft_FldObj_Twist_AreaBridge":
                    type = typeof(Lft_FldObj_Twist_AreaBridge);
                    break;

                case "Lft_FldObj_Twist_AreaMushroom":
                    type = typeof(Lft_FldObj_Twist_AreaMushroom);
                    break;

                case "Lft_FldObj_Twist_AreaRespawn":
                    type = typeof(Lft_FldObj_Twist_AreaRespawn);
                    break;

                case "Lft_FldObj_Twist_AreaSide":
                    type = typeof(Lft_FldObj_Twist_AreaSide);
                    break;

                case "Lft_FldObj_Twist_BridgeFlatArea":
                    type = typeof(Lft_FldObj_Twist_BridgeFlatArea);
                    break;

                case "Lft_FldObj_Twist_BridgeFlatHoko":
                    type = typeof(Lft_FldObj_Twist_BridgeFlatHoko);
                    break;

                case "Lft_FldObj_Twist_CenterSlope":
                    type = typeof(Lft_FldObj_Twist_CenterSlope);
                    break;

                case "Lft_FldObj_Twist_Edge":
                    type = typeof(Lft_FldObj_Twist_Edge);
                    break;

                case "Lft_FldObj_Twist_HokoBridge":
                    type = typeof(Lft_FldObj_Twist_HokoBridge);
                    break;

                case "Lft_FldObj_Twist_HokoFence":
                    type = typeof(Lft_FldObj_Twist_HokoFence);
                    break;

                case "Lft_FldObj_Twist_HokoMushroom":
                    type = typeof(Lft_FldObj_Twist_HokoMushroom);
                    break;

                case "Lft_FldObj_Twist_HokoPillar":
                    type = typeof(Lft_FldObj_Twist_HokoPillar);
                    break;

                case "Lft_FldObj_Twist_PaintBridge":
                    type = typeof(Lft_FldObj_Twist_PaintBridge);
                    break;

                case "Lft_FldObj_Twist_PaintSlope":
                    type = typeof(Lft_FldObj_Twist_PaintSlope);
                    break;

                case "Lft_FldObj_Twist_Pillar":
                    type = typeof(Lft_FldObj_Twist_Pillar);
                    break;

                case "Lft_FldObj_Twist_WallArea":
                    type = typeof(Lft_FldObj_Twist_WallArea);
                    break;

                case "Lft_FldObj_UplandArtY_base":
                    type = typeof(Lft_FldObj_UplandArtY_base);
                    break;

                case "Lft_FldObj_UplandArtY_top":
                    type = typeof(Lft_FldObj_UplandArtY_top);
                    break;

                case "Lft_FldObj_UplandAsariSet":
                    type = typeof(Lft_FldObj_UplandAsariSet);
                    break;

                case "Lft_FldObj_UplandAsariSet2":
                    type = typeof(Lft_FldObj_UplandAsariSet2);
                    break;

                case "Lft_FldObj_UplandHokoSet":
                    type = typeof(Lft_FldObj_UplandHokoSet);
                    break;

                case "Lft_FldObj_UplandIkagaeshi":
                    type = typeof(Lft_FldObj_UplandIkagaeshi);
                    break;

                case "Lft_FldObj_UplandNoBR":
                    type = typeof(Lft_FldObj_UplandNoBR);
                    break;

                case "Lft_FldObj_UplandPartitionPair_Vcl":
                    type = typeof(Lft_FldObj_UplandPartitionPair_Vcl);
                    break;

                case "Lft_FldObj_UplandSculptureSingle":
                    type = typeof(Lft_FldObj_UplandSculptureSingle);
                    break;

                case "Lft_FldObj_UplandSideBox":
                    type = typeof(Lft_FldObj_UplandSideBox);
                    break;

                case "Lft_FldObj_UplandTree":
                    type = typeof(Lft_FldObj_UplandTree);
                    break;

                case "Lft_FldObj_UplandWallBlockSet":
                    type = typeof(Lft_FldObj_UplandWallBlockSet);
                    break;

                case "Lft_FldObj_Wave03FlagBlue00":
                    type = typeof(Lft_FldObj_Wave03FlagBlue00);
                    break;

                case "Lft_FldObj_WaveAsariSlope":
                    type = typeof(Lft_FldObj_WaveAsariSlope);
                    break;

                case "Lft_FldObj_WaveFloor00":
                    type = typeof(Lft_FldObj_WaveFloor00);
                    break;

                case "Lft_FldObj_WaveFloor01":
                    type = typeof(Lft_FldObj_WaveFloor01);
                    break;

                case "Lft_FldObj_WaveRightPnt":
                    type = typeof(Lft_FldObj_WaveRightPnt);
                    break;

                case "Lft_FldObj_WaveRightVcl":
                    type = typeof(Lft_FldObj_WaveRightVcl);
                    break;

                case "Lft_FldObj_WaveRightVgl":
                    type = typeof(Lft_FldObj_WaveRightVgl);
                    break;

                case "Lft_FldObj_WaveSideHako":
                    type = typeof(Lft_FldObj_WaveSideHako);
                    break;

                case "Lft_FldObj_WaveSideTower00":
                    type = typeof(Lft_FldObj_WaveSideTower00);
                    break;

                case "Lft_FldObj_WaveSideTower01":
                    type = typeof(Lft_FldObj_WaveSideTower01);
                    break;

                case "Lft_FldObj_WaveSlope":
                    type = typeof(Lft_FldObj_WaveSlope);
                    break;

                case "Lft_FldObj_WaveWall00":
                    type = typeof(Lft_FldObj_WaveWall00);
                    break;

                case "Lft_FldObj_WaveWall01":
                    type = typeof(Lft_FldObj_WaveWall01);
                    break;

                case "Lft_FldObj_YagaraCircleRamp":
                    type = typeof(Lft_FldObj_YagaraCircleRamp);
                    break;

                case "Lft_FldObj_YagaraTentWallHoko":
                    type = typeof(Lft_FldObj_YagaraTentWallHoko);
                    break;

                case "Lft_FldObj_Yagara_Charger":
                    type = typeof(Lft_FldObj_Yagara_Charger);
                    break;

                case "Lft_FldObj_Yagara_PackPnt":
                    type = typeof(Lft_FldObj_Yagara_PackPnt);
                    break;

                case "Lft_FldObj_Yagara_PackTcl":
                    type = typeof(Lft_FldObj_Yagara_PackTcl);
                    break;

                case "Lft_FldObj_Yagara_PackVar":
                    type = typeof(Lft_FldObj_Yagara_PackVar);
                    break;

                case "Lft_FldObj_Yagara_PackVcl":
                    type = typeof(Lft_FldObj_Yagara_PackVcl);
                    break;

                case "Lft_FldObj_Yagara_PackVgl":
                    type = typeof(Lft_FldObj_Yagara_PackVgl);
                    break;

                case "Lft_FldObj_Yagara_PackVlf":
                    type = typeof(Lft_FldObj_Yagara_PackVlf);
                    break;

                case "Lft_FldObj_Yagara_Paint":
                    type = typeof(Lft_FldObj_Yagara_Paint);
                    break;

                case "Lft_FldObj_Yagara_Tentside":
                    type = typeof(Lft_FldObj_Yagara_Tentside);
                    break;

                case "Lft_FldObj_YunohanaLeftBoxVlf":
                    type = typeof(Lft_FldObj_YunohanaLeftBoxVlf);
                    break;

                case "Lft_FldObj_YunohanaLeftFence01":
                    type = typeof(Lft_FldObj_YunohanaLeftFence01);
                    break;

                case "Lft_FldObj_YunohanaLeftFence02":
                    type = typeof(Lft_FldObj_YunohanaLeftFence02);
                    break;

                case "Lft_FldObj_YunohanaLeftSlopeVar":
                    type = typeof(Lft_FldObj_YunohanaLeftSlopeVar);
                    break;

                case "Lft_FldObj_YunohanaLeftSlopeVgl":
                    type = typeof(Lft_FldObj_YunohanaLeftSlopeVgl);
                    break;

                case "Lft_FldObj_YunohanaLeftStageLarge":
                    type = typeof(Lft_FldObj_YunohanaLeftStageLarge);
                    break;

                case "Lft_FldObj_YunohanaLeftStageSmall":
                    type = typeof(Lft_FldObj_YunohanaLeftStageSmall);
                    break;

                case "Lft_FldObj_YunohanaLeftStageVcl":
                    type = typeof(Lft_FldObj_YunohanaLeftStageVcl);
                    break;

                case "Lft_FldObj_YunohanaSlideBox":
                    type = typeof(Lft_FldObj_YunohanaSlideBox);
                    break;

                case "Lft_FldObj_YunohanaTowerFence01":
                    type = typeof(Lft_FldObj_YunohanaTowerFence01);
                    break;

                case "Lft_FldObj_YunohanaTowerFence02":
                    type = typeof(Lft_FldObj_YunohanaTowerFence02);
                    break;

                case "Lft_FldObj_YunohanaTowerFence03":
                    type = typeof(Lft_FldObj_YunohanaTowerFence03);
                    break;

                case "Lft_FldObj_YunohanaTowerHigh":
                    type = typeof(Lft_FldObj_YunohanaTowerHigh);
                    break;

                case "Lft_FldObj_YunohanaTowerLow":
                    type = typeof(Lft_FldObj_YunohanaTowerLow);
                    break;

                case "Lft_FldObj_Yunohana_PackPnt":
                    type = typeof(Lft_FldObj_Yunohana_PackPnt);
                    break;

                case "Lft_FldObj_Yunohana_PackVar":
                    type = typeof(Lft_FldObj_Yunohana_PackVar);
                    break;

                case "Lft_FldObj_Yunohana_PackVcl":
                    type = typeof(Lft_FldObj_Yunohana_PackVcl);
                    break;

                case "Lft_FldObj_Yunohana_PackVgl":
                    type = typeof(Lft_FldObj_Yunohana_PackVgl);
                    break;

                case "Lft_FldObj_Yunohana_PackVlf":
                    type = typeof(Lft_FldObj_Yunohana_PackVlf);
                    break;

                case "Lft_Fld_BigRunTemple00_GeneralBox":
                    type = typeof(Lft_Fld_BigRunTemple00_GeneralBox);
                    break;

                case "Lft_Fld_BigWorldLaunchPad00":
                    type = typeof(Lft_Fld_BigWorldLaunchPad00);
                    break;

                case "Lft_Fld_BigWorldLaunchPad01":
                    type = typeof(Lft_Fld_BigWorldLaunchPad01);
                    break;

                case "Lft_Fld_BigWorldLaunchPad02":
                    type = typeof(Lft_Fld_BigWorldLaunchPad02);
                    break;

                case "Lft_Fld_BigWorldLaunchPad03":
                    type = typeof(Lft_Fld_BigWorldLaunchPad03);
                    break;

                case "Lft_Fld_BigWorldLaunchPad04":
                    type = typeof(Lft_Fld_BigWorldLaunchPad04);
                    break;

                case "Lft_Fld_BigWorld_Area02":
                    type = typeof(Lft_Fld_BigWorld_Area02);
                    break;

                case "Lft_Fld_BigWorld_Area03":
                    type = typeof(Lft_Fld_BigWorld_Area03);
                    break;

                case "Lft_Fld_BigWorld_Area04":
                    type = typeof(Lft_Fld_BigWorld_Area04);
                    break;

                case "Lft_Fld_BigWorld_Area05":
                    type = typeof(Lft_Fld_BigWorld_Area05);
                    break;

                case "Lft_Fld_BigWorld_Area06":
                    type = typeof(Lft_Fld_BigWorld_Area06);
                    break;

                case "Lft_Fld_BridgeCeiling":
                    type = typeof(Lft_Fld_BridgeCeiling);
                    break;

                case "Lft_Fld_CafeBuilding":
                    type = typeof(Lft_Fld_CafeBuilding);
                    break;

                case "Lft_Fld_CoopLobbyBuilding":
                    type = typeof(Lft_Fld_CoopLobbyBuilding);
                    break;

                case "Lft_Fld_Cross00_GeneralCube_Pnt":
                    type = typeof(Lft_Fld_Cross00_GeneralCube_Pnt);
                    break;

                case "Lft_Fld_Cross00_GeneralCube_Tcl":
                    type = typeof(Lft_Fld_Cross00_GeneralCube_Tcl);
                    break;

                case "Lft_Fld_Cross00_GeneralCube_Var":
                    type = typeof(Lft_Fld_Cross00_GeneralCube_Var);
                    break;

                case "Lft_Fld_Cross00_GeneralCube_Vcl":
                    type = typeof(Lft_Fld_Cross00_GeneralCube_Vcl);
                    break;

                case "Lft_Fld_Cross00_GeneralCube_Vgl":
                    type = typeof(Lft_Fld_Cross00_GeneralCube_Vgl);
                    break;

                case "Lft_Fld_Cross00_GeneralCube_Vlf":
                    type = typeof(Lft_Fld_Cross00_GeneralCube_Vlf);
                    break;

                case "Lft_Fld_Cross00_Tclobj":
                    type = typeof(Lft_Fld_Cross00_Tclobj);
                    break;

                case "Lft_Fld_Cross00_Var":
                    type = typeof(Lft_Fld_Cross00_Var);
                    break;

                case "Lft_Fld_Cross00_Vcl":
                    type = typeof(Lft_Fld_Cross00_Vcl);
                    break;

                case "Lft_Fld_Cross00_Vgl":
                    type = typeof(Lft_Fld_Cross00_Vgl);
                    break;

                case "Lft_Fld_Cross00_Vlf":
                    type = typeof(Lft_Fld_Cross00_Vlf);
                    break;

                case "Lft_Fld_LaunchPadTube":
                    type = typeof(Lft_Fld_LaunchPadTube);
                    break;

                case "Lft_Fld_LockerEditBG":
                    type = typeof(Lft_Fld_LockerEditBG);
                    break;

                case "Lft_Fld_Pillar03_Tape":
                    type = typeof(Lft_Fld_Pillar03_Tape);
                    break;

                case "Lft_Fld_PlayerMakeBrokenTower":
                    type = typeof(Lft_Fld_PlayerMakeBrokenTower);
                    break;

                case "Lft_Fld_RocketSiloHatch":
                    type = typeof(Lft_Fld_RocketSiloHatch);
                    break;

                case "Lft_Fld_RocketSiloHatchRingOnly":
                    type = typeof(Lft_Fld_RocketSiloHatchRingOnly);
                    break;

                case "Lft_Fld_Room_News":
                    type = typeof(Lft_Fld_Room_News);
                    break;

                case "Lft_Fld_SdodrBarrierKingOdako2ndBG":
                    type = typeof(Lft_Fld_SdodrBarrierKingOdako2ndBG);
                    break;

                case "Lft_Fld_ShopExteriorWeapon":
                    type = typeof(Lft_Fld_ShopExteriorWeapon);
                    break;

                case "Lft_Fld_Spider00_3mBlock":
                    type = typeof(Lft_Fld_Spider00_3mBlock);
                    break;

                case "Lft_Fld_Spider00_CenterSideFence":
                    type = typeof(Lft_Fld_Spider00_CenterSideFence);
                    break;

                case "Lft_Fld_Spider00_Pole":
                    type = typeof(Lft_Fld_Spider00_Pole);
                    break;

                case "Lft_Fld_Spider00_RailBlock":
                    type = typeof(Lft_Fld_Spider00_RailBlock);
                    break;

                case "Lft_Fld_Spider00_RespawnLeftTakadai":
                    type = typeof(Lft_Fld_Spider00_RespawnLeftTakadai);
                    break;

                case "Lft_Fld_Spider00_RightSignClose":
                    type = typeof(Lft_Fld_Spider00_RightSignClose);
                    break;

                case "Lft_Fld_Spider00_SideSlope":
                    type = typeof(Lft_Fld_Spider00_SideSlope);
                    break;

                case "Lft_Fld_Spider00_Slope":
                    type = typeof(Lft_Fld_Spider00_Slope);
                    break;

                case "Lft_Fld_Spider00_TclSet":
                    type = typeof(Lft_Fld_Spider00_TclSet);
                    break;

                case "Lft_Fld_Spider00_VarCenterFence":
                    type = typeof(Lft_Fld_Spider00_VarCenterFence);
                    break;

                case "Lft_Fld_Spider00_VarChargerYoke":
                    type = typeof(Lft_Fld_Spider00_VarChargerYoke);
                    break;

                case "Lft_Fld_Spider00_VarSet":
                    type = typeof(Lft_Fld_Spider00_VarSet);
                    break;

                case "Lft_Fld_Spider00_VarSlopeTakadai":
                    type = typeof(Lft_Fld_Spider00_VarSlopeTakadai);
                    break;

                case "Lft_Fld_Spider00_VclSlope":
                    type = typeof(Lft_Fld_Spider00_VclSlope);
                    break;

                case "Lft_Fld_Spider00_VglSet":
                    type = typeof(Lft_Fld_Spider00_VglSet);
                    break;

                case "Lft_Fld_Spider00_VglSlopeFloor":
                    type = typeof(Lft_Fld_Spider00_VglSlopeFloor);
                    break;

                case "Lft_Fld_Spider00_VlfCenter":
                    type = typeof(Lft_Fld_Spider00_VlfCenter);
                    break;

                case "Lft_Fld_Spider00_VlfCenterFence":
                    type = typeof(Lft_Fld_Spider00_VlfCenterFence);
                    break;

                case "Lft_Fld_Spider00_VlfFloor":
                    type = typeof(Lft_Fld_Spider00_VlfFloor);
                    break;

                case "Lft_Fld_Spider00_VlfSet":
                    type = typeof(Lft_Fld_Spider00_VlfSet);
                    break;

                case "Lft_Fld_Spider00_VSgame":
                    type = typeof(Lft_Fld_Spider00_VSgame);
                    break;

                case "Lft_Fld_VSLobbyExterior":
                    type = typeof(Lft_Fld_VSLobbyExterior);
                    break;

                case "Lft_GeneralBoxHalfPaint_30x30x30_Blitz":
                    type = typeof(Lft_GeneralBoxHalfPaint_30x30x30_Blitz);
                    break;

                case "Lft_GeneralCube15x15x15AP":
                    type = typeof(Lft_GeneralCube15x15x15AP);
                    break;

                case "Lft_GeneralCube15x15x15NP":
                    type = typeof(Lft_GeneralCube15x15x15NP);
                    break;

                case "Lft_GeneralCube15x30x15NP":
                    type = typeof(Lft_GeneralCube15x30x15NP);
                    break;

                case "Lft_GeneralCube15x45x30NP":
                    type = typeof(Lft_GeneralCube15x45x30NP);
                    break;

                case "Lft_GeneralCube30x15x15AP":
                    type = typeof(Lft_GeneralCube30x15x15AP);
                    break;

                case "Lft_GeneralCube30x15x15NP":
                    type = typeof(Lft_GeneralCube30x15x15NP);
                    break;

                case "Lft_GeneralCube30x15x30AP":
                    type = typeof(Lft_GeneralCube30x15x30AP);
                    break;

                case "Lft_GeneralCube30x15x45AP":
                    type = typeof(Lft_GeneralCube30x15x45AP);
                    break;

                case "Lft_GeneralCube30x15x80AP":
                    type = typeof(Lft_GeneralCube30x15x80AP);
                    break;

                case "Lft_GeneralCube30x30x15AP":
                    type = typeof(Lft_GeneralCube30x30x15AP);
                    break;

                case "Lft_GeneralCube30x30x15NP":
                    type = typeof(Lft_GeneralCube30x30x15NP);
                    break;

                case "Lft_GeneralCube30x30x30AP":
                    type = typeof(Lft_GeneralCube30x30x30AP);
                    break;

                case "Lft_GeneralCube30x30x45AP":
                    type = typeof(Lft_GeneralCube30x30x45AP);
                    break;

                case "Lft_GeneralCube30x45x15AP":
                    type = typeof(Lft_GeneralCube30x45x15AP);
                    break;

                case "Lft_GeneralCube30x45x30AP":
                    type = typeof(Lft_GeneralCube30x45x30AP);
                    break;

                case "Lft_GeneralCube30x45x30NP":
                    type = typeof(Lft_GeneralCube30x45x30NP);
                    break;

                case "Lft_GeneralCube40x30x15AP":
                    type = typeof(Lft_GeneralCube40x30x15AP);
                    break;

                case "Lft_GeneralCube60x30x45AP":
                    type = typeof(Lft_GeneralCube60x30x45AP);
                    break;

                case "Lft_GeneralSlope30x30x60AP":
                    type = typeof(Lft_GeneralSlope30x30x60AP);
                    break;

                case "Lft_GeneralSlope60x25x45AP":
                    type = typeof(Lft_GeneralSlope60x25x45AP);
                    break;

                case "Lft_GeneralSlope70x30x60AP":
                    type = typeof(Lft_GeneralSlope70x30x60AP);
                    break;

                case "Lft_GeneralSlope_60x30x60AP":
                    type = typeof(Lft_GeneralSlope_60x30x60AP);
                    break;

                case "Lft_HokoSignBoard":
                    type = typeof(Lft_HokoSignBoard);
                    break;

                case "Lft_HokoSignBoard_Narrow":
                    type = typeof(Lft_HokoSignBoard_Narrow);
                    break;

                case "Lft_IkanikeLongPivot":
                    type = typeof(Lft_IkanikeLongPivot);
                    break;

                case "Lft_IkanikePivot":
                    type = typeof(Lft_IkanikePivot);
                    break;

                case "Lft_KeepOutBulletSdodr":
                    type = typeof(Lft_KeepOutBulletSdodr);
                    break;

                case "Lft_KeepOutEnemy":
                    type = typeof(Lft_KeepOutEnemy);
                    break;

                case "Lft_KeepOutEnemyLarge":
                    type = typeof(Lft_KeepOutEnemyLarge);
                    break;

                case "Lft_KeepOutNPC":
                    type = typeof(Lft_KeepOutNPC);
                    break;

                case "Lft_KeepOutPlayer":
                    type = typeof(Lft_KeepOutPlayer);
                    break;

                case "Lft_KeepOutRival":
                    type = typeof(Lft_KeepOutRival);
                    break;

                case "Lft_KeepOutVictoryBallSdodr":
                    type = typeof(Lft_KeepOutVictoryBallSdodr);
                    break;

                case "Lft_KumaRocket":
                    type = typeof(Lft_KumaRocket);
                    break;

                case "Lft_LineCircleHangingSign":
                    type = typeof(Lft_LineCircleHangingSign);
                    break;

                case "Lft_LineRespwanfield":
                    type = typeof(Lft_LineRespwanfield);
                    break;

                case "Lft_LineYaguraKabe":
                    type = typeof(Lft_LineYaguraKabe);
                    break;

                case "Lft_MikoshiBody":
                    type = typeof(Lft_MikoshiBody);
                    break;

                case "Lft_MikoshiBodyConstruction":
                    type = typeof(Lft_MikoshiBodyConstruction);
                    break;

                case "Lft_MikoshiHead":
                    type = typeof(Lft_MikoshiHead);
                    break;

                case "Lft_MikoshiHeadConstruction":
                    type = typeof(Lft_MikoshiHeadConstruction);
                    break;

                case "Lft_MikoshiStage":
                    type = typeof(Lft_MikoshiStage);
                    break;

                case "Lft_MikoshiTail":
                    type = typeof(Lft_MikoshiTail);
                    break;

                case "Lft_MikoshiTailConstruction":
                    type = typeof(Lft_MikoshiTailConstruction);
                    break;

                case "Lft_Motorcycle":
                    type = typeof(Lft_Motorcycle);
                    break;

                case "Lft_MptBase":
                    type = typeof(Lft_MptBase);
                    break;

                case "Lft_MsnBlowoutsBaseCube40x20x20NP":
                    type = typeof(Lft_MsnBlowoutsBaseCube40x20x20NP);
                    break;

                case "Lft_MsnCrossFloor180x15x180":
                    type = typeof(Lft_MsnCrossFloor180x15x180);
                    break;

                case "Lft_MsnCube105x15x3Fence":
                    type = typeof(Lft_MsnCube105x15x3Fence);
                    break;

                case "Lft_MsnCube10x30x10NP":
                    type = typeof(Lft_MsnCube10x30x10NP);
                    break;

                case "Lft_MsnCube10x30x180NP":
                    type = typeof(Lft_MsnCube10x30x180NP);
                    break;

                case "Lft_MsnCube10x30x60NP":
                    type = typeof(Lft_MsnCube10x30x60NP);
                    break;

                case "Lft_MsnCube120x100x120AP":
                    type = typeof(Lft_MsnCube120x100x120AP);
                    break;

                case "Lft_MsnCube120x50x120AP":
                    type = typeof(Lft_MsnCube120x50x120AP);
                    break;

                case "Lft_MsnCube15x120x15NP":
                    type = typeof(Lft_MsnCube15x120x15NP);
                    break;

                case "Lft_MsnCube15x15x3Fence":
                    type = typeof(Lft_MsnCube15x15x3Fence);
                    break;

                case "Lft_MsnCube165x15x3Fence":
                    type = typeof(Lft_MsnCube165x15x3Fence);
                    break;

                case "Lft_MsnCube180x5x5NP":
                    type = typeof(Lft_MsnCube180x5x5NP);
                    break;

                case "Lft_MsnCube20x25x20AP":
                    type = typeof(Lft_MsnCube20x25x20AP);
                    break;

                case "Lft_MsnCube30x30x10Glass":
                    type = typeof(Lft_MsnCube30x30x10Glass);
                    break;

                case "Lft_MsnCube30x30x30AP":
                    type = typeof(Lft_MsnCube30x30x30AP);
                    break;

                case "Lft_MsnCube30x60x30XP":
                    type = typeof(Lft_MsnCube30x60x30XP);
                    break;

                case "Lft_MsnCube3x30x60Fence":
                    type = typeof(Lft_MsnCube3x30x60Fence);
                    break;

                case "Lft_MsnCube3x60x60Fence":
                    type = typeof(Lft_MsnCube3x60x60Fence);
                    break;

                case "Lft_MsnCube40x40x40AP":
                    type = typeof(Lft_MsnCube40x40x40AP);
                    break;

                case "Lft_MsnCube45x15x3Fence":
                    type = typeof(Lft_MsnCube45x15x3Fence);
                    break;

                case "Lft_MsnCube60x10x60Glass":
                    type = typeof(Lft_MsnCube60x10x60Glass);
                    break;

                case "Lft_MsnCube60x40x60AP":
                    type = typeof(Lft_MsnCube60x40x60AP);
                    break;

                case "Lft_MsnCube60x5x5NP":
                    type = typeof(Lft_MsnCube60x5x5NP);
                    break;

                case "Lft_MsnCube60x5x5NPRobo":
                    type = typeof(Lft_MsnCube60x5x5NPRobo);
                    break;

                case "Lft_MsnCube60x60x10Glass":
                    type = typeof(Lft_MsnCube60x60x10Glass);
                    break;

                case "Lft_MsnCube60x60x180AP":
                    type = typeof(Lft_MsnCube60x60x180AP);
                    break;

                case "Lft_MsnCube60x60x60AP":
                    type = typeof(Lft_MsnCube60x60x60AP);
                    break;

                case "Lft_MsnCube60x60x60NP":
                    type = typeof(Lft_MsnCube60x60x60NP);
                    break;

                case "Lft_MsnCube60x90x15Glass":
                    type = typeof(Lft_MsnCube60x90x15Glass);
                    break;

                case "Lft_MsnCube75x15x3Fence":
                    type = typeof(Lft_MsnCube75x15x3Fence);
                    break;

                case "Lft_MsnCube90x90x10Glass":
                    type = typeof(Lft_MsnCube90x90x10Glass);
                    break;

                case "Lft_MsnCylinder15x15x15AP":
                    type = typeof(Lft_MsnCylinder15x15x15AP);
                    break;

                case "Lft_MsnEdge105x20x3Fence":
                    type = typeof(Lft_MsnEdge105x20x3Fence);
                    break;

                case "Lft_MsnEdge15x20x3Fence":
                    type = typeof(Lft_MsnEdge15x20x3Fence);
                    break;

                case "Lft_MsnEdge165x20x3Fence":
                    type = typeof(Lft_MsnEdge165x20x3Fence);
                    break;

                case "Lft_MsnEdge45x20x3Fence":
                    type = typeof(Lft_MsnEdge45x20x3Fence);
                    break;

                case "Lft_MsnEdge75x20x3Fence":
                    type = typeof(Lft_MsnEdge75x20x3Fence);
                    break;

                case "Lft_MsnFloor120x15x120AP":
                    type = typeof(Lft_MsnFloor120x15x120AP);
                    break;

                case "Lft_MsnFloor120x15x180AP":
                    type = typeof(Lft_MsnFloor120x15x180AP);
                    break;

                case "Lft_MsnFloor180x15x180AP":
                    type = typeof(Lft_MsnFloor180x15x180AP);
                    break;

                case "Lft_MsnFloor180x15x60AP":
                    type = typeof(Lft_MsnFloor180x15x60AP);
                    break;

                case "Lft_MsnFloor180x15x90AP":
                    type = typeof(Lft_MsnFloor180x15x90AP);
                    break;

                case "Lft_MsnFloor180x40x60XP":
                    type = typeof(Lft_MsnFloor180x40x60XP);
                    break;

                case "Lft_MsnFloor300x15x300AP":
                    type = typeof(Lft_MsnFloor300x15x300AP);
                    break;

                case "Lft_MsnFloor30x15x180AP":
                    type = typeof(Lft_MsnFloor30x15x180AP);
                    break;

                case "Lft_MsnFloor30x3x30Fence":
                    type = typeof(Lft_MsnFloor30x3x30Fence);
                    break;

                case "Lft_MsnFloor360x15x180NP":
                    type = typeof(Lft_MsnFloor360x15x180NP);
                    break;

                case "Lft_MsnFloor60x100x180AP":
                    type = typeof(Lft_MsnFloor60x100x180AP);
                    break;

                case "Lft_MsnFloor60x15x120AP":
                    type = typeof(Lft_MsnFloor60x15x120AP);
                    break;

                case "Lft_MsnFloor60x15x15TP":
                    type = typeof(Lft_MsnFloor60x15x15TP);
                    break;

                case "Lft_MsnFloor60x15x180AP":
                    type = typeof(Lft_MsnFloor60x15x180AP);
                    break;

                case "Lft_MsnFloor60x15x30AP":
                    type = typeof(Lft_MsnFloor60x15x30AP);
                    break;

                case "Lft_MsnFloor60x15x60AP":
                    type = typeof(Lft_MsnFloor60x15x60AP);
                    break;

                case "Lft_MsnFloor60x15x60NP":
                    type = typeof(Lft_MsnFloor60x15x60NP);
                    break;

                case "Lft_MsnFloor60x15x60TP":
                    type = typeof(Lft_MsnFloor60x15x60TP);
                    break;

                case "Lft_MsnFloor60x15x90AP":
                    type = typeof(Lft_MsnFloor60x15x90AP);
                    break;

                case "Lft_MsnFloor60x3x30Fence":
                    type = typeof(Lft_MsnFloor60x3x30Fence);
                    break;

                case "Lft_MsnFloor60x5x40Fence":
                    type = typeof(Lft_MsnFloor60x5x40Fence);
                    break;

                case "Lft_MsnFloor60x5x60Fence":
                    type = typeof(Lft_MsnFloor60x5x60Fence);
                    break;

                case "Lft_MsnFloor90x15x120AP":
                    type = typeof(Lft_MsnFloor90x15x120AP);
                    break;

                case "Lft_MsnFloor90x15x180NP":
                    type = typeof(Lft_MsnFloor90x15x180NP);
                    break;

                case "Lft_MsnFloor90x15x90AP":
                    type = typeof(Lft_MsnFloor90x15x90AP);
                    break;

                case "Lft_MsnFloor90x15x90NP":
                    type = typeof(Lft_MsnFloor90x15x90NP);
                    break;

                case "Lft_MsnFloor90x3x30Fence":
                    type = typeof(Lft_MsnFloor90x3x30Fence);
                    break;

                case "Lft_MsnGateL150x180x20NP":
                    type = typeof(Lft_MsnGateL150x180x20NP);
                    break;

                case "Lft_MsnGateR150x180x20NP":
                    type = typeof(Lft_MsnGateR150x180x20NP);
                    break;

                case "Lft_MsnGoalFloor180x30x90AP":
                    type = typeof(Lft_MsnGoalFloor180x30x90AP);
                    break;

                case "Lft_MsnGoalGateNP":
                    type = typeof(Lft_MsnGoalGateNP);
                    break;

                case "Lft_MsnHoleFloor180x15x180AP":
                    type = typeof(Lft_MsnHoleFloor180x15x180AP);
                    break;

                case "Lft_MsnHoleFloor300x15x300AP":
                    type = typeof(Lft_MsnHoleFloor300x15x300AP);
                    break;

                case "Lft_MsnOctagonFloor180x15x180AP":
                    type = typeof(Lft_MsnOctagonFloor180x15x180AP);
                    break;

                case "Lft_MsnOctagonFloor240x15x240AP":
                    type = typeof(Lft_MsnOctagonFloor240x15x240AP);
                    break;

                case "Lft_MsnOctagonFloor60x15x60AP":
                    type = typeof(Lft_MsnOctagonFloor60x15x60AP);
                    break;

                case "Lft_MsnOctagonPiller120x180x120AP":
                    type = typeof(Lft_MsnOctagonPiller120x180x120AP);
                    break;

                case "Lft_MsnOctagonPiller15x15x15AP":
                    type = typeof(Lft_MsnOctagonPiller15x15x15AP);
                    break;

                case "Lft_MsnOctagonPiller15x60x15AP":
                    type = typeof(Lft_MsnOctagonPiller15x60x15AP);
                    break;

                case "Lft_MsnOctagonPiller30x30x30AP":
                    type = typeof(Lft_MsnOctagonPiller30x30x30AP);
                    break;

                case "Lft_MsnOctagonPiller30x90x0NP":
                    type = typeof(Lft_MsnOctagonPiller30x90x0NP);
                    break;

                case "Lft_MsnOctagonPiller30x90x30AP":
                    type = typeof(Lft_MsnOctagonPiller30x90x30AP);
                    break;

                case "Lft_MsnOctagonPiller30x90x30NP":
                    type = typeof(Lft_MsnOctagonPiller30x90x30NP);
                    break;

                case "Lft_MsnOctagonPiller60x180x60AP":
                    type = typeof(Lft_MsnOctagonPiller60x180x60AP);
                    break;

                case "Lft_MsnOctagonPiller60x60x60AP":
                    type = typeof(Lft_MsnOctagonPiller60x60x60AP);
                    break;

                case "Lft_MsnPedestal60x20x60AP":
                    type = typeof(Lft_MsnPedestal60x20x60AP);
                    break;

                case "Lft_MsnPedestal60x5x60AP":
                    type = typeof(Lft_MsnPedestal60x5x60AP);
                    break;

                case "Lft_MsnPiller40x600x40NP":
                    type = typeof(Lft_MsnPiller40x600x40NP);
                    break;

                case "Lft_MsnPylamid15x15x15NP":
                    type = typeof(Lft_MsnPylamid15x15x15NP);
                    break;

                case "Lft_MsnPylamid15x15x30NP":
                    type = typeof(Lft_MsnPylamid15x15x30NP);
                    break;

                case "Lft_MsnSlope150x60x30AP":
                    type = typeof(Lft_MsnSlope150x60x30AP);
                    break;

                case "Lft_MsnSlope60x60x150AP":
                    type = typeof(Lft_MsnSlope60x60x150AP);
                    break;

                case "Lft_MsnSlope90x60x150AP":
                    type = typeof(Lft_MsnSlope90x60x150AP);
                    break;

                case "Lft_MsnTriangleFloor30x15x30":
                    type = typeof(Lft_MsnTriangleFloor30x15x30);
                    break;

                case "Lft_MsnTriangleFloor60x15x60":
                    type = typeof(Lft_MsnTriangleFloor60x15x60);
                    break;

                case "Lft_MsnTriangleFloor90x15x90":
                    type = typeof(Lft_MsnTriangleFloor90x15x90);
                    break;

                case "Lft_Obj_VendingMachine":
                    type = typeof(Lft_Obj_VendingMachine);
                    break;

                case "Lft_PipelineArea01":
                    type = typeof(Lft_PipelineArea01);
                    break;

                case "Lft_PipelineArea02":
                    type = typeof(Lft_PipelineArea02);
                    break;

                case "Lft_PipelineArea03":
                    type = typeof(Lft_PipelineArea03);
                    break;

                case "Lft_PipelineArea04":
                    type = typeof(Lft_PipelineArea04);
                    break;

                case "Lft_PipelineArea05":
                    type = typeof(Lft_PipelineArea05);
                    break;

                case "Lft_PipelineArea06":
                    type = typeof(Lft_PipelineArea06);
                    break;

                case "Lft_PipelineArea07":
                    type = typeof(Lft_PipelineArea07);
                    break;

                case "Lft_PivotArea":
                    type = typeof(Lft_PivotArea);
                    break;

                case "Lft_PivotDoor03":
                    type = typeof(Lft_PivotDoor03);
                    break;

                case "Lft_PivotDoorArea00":
                    type = typeof(Lft_PivotDoorArea00);
                    break;

                case "Lft_PivotDoorHoko":
                    type = typeof(Lft_PivotDoorHoko);
                    break;

                case "Lft_PivotDoorYagura":
                    type = typeof(Lft_PivotDoorYagura);
                    break;

                case "Lft_PivotHoko":
                    type = typeof(Lft_PivotHoko);
                    break;

                case "Lft_PivotPnt":
                    type = typeof(Lft_PivotPnt);
                    break;

                case "Lft_Shakelift00":
                    type = typeof(Lft_Shakelift00);
                    break;

                case "Lft_Shakeship00":
                    type = typeof(Lft_Shakeship00);
                    break;

                case "Lft_UplandHokoWall":
                    type = typeof(Lft_UplandHokoWall);
                    break;

                case "Lft_UplandNet":
                    type = typeof(Lft_UplandNet);
                    break;

                case "Lft_UplandNetStep":
                    type = typeof(Lft_UplandNetStep);
                    break;

                case "Lft_UplandPartition":
                    type = typeof(Lft_UplandPartition);
                    break;

                case "Lft_UplandPartitionPair":
                    type = typeof(Lft_UplandPartitionPair);
                    break;

                case "Lft_UplandSculpture":
                    type = typeof(Lft_UplandSculpture);
                    break;

                case "Lft_YagaraArea":
                    type = typeof(Lft_YagaraArea);
                    break;

                case "Lft_YagaraFence":
                    type = typeof(Lft_YagaraFence);
                    break;

                case "Lft_YagaraSide":
                    type = typeof(Lft_YagaraSide);
                    break;

                case "Lft_YagaraSidefield00":
                    type = typeof(Lft_YagaraSidefield00);
                    break;

                case "Lft_YagaraSidefield01":
                    type = typeof(Lft_YagaraSidefield01);
                    break;

                case "Lft_YagaraSlope":
                    type = typeof(Lft_YagaraSlope);
                    break;

                case "Lft_YagaraSlopefence00":
                    type = typeof(Lft_YagaraSlopefence00);
                    break;

                case "Lft_YagaraSlopefence01":
                    type = typeof(Lft_YagaraSlopefence01);
                    break;

                case "Lft_YagaraWall01":
                    type = typeof(Lft_YagaraWall01);
                    break;

                case "LobbyTurnDirectionArea":
                    type = typeof(LobbyTurnDirectionArea);
                    break;

                case "LocatorAreaSwitchOnlineForAnyPlayer":
                    type = typeof(LocatorAreaSwitchOnlineForAnyPlayer);
                    break;

                case "LocatorBulletBlastCrossPaintChangeArea":
                    type = typeof(LocatorBulletBlastCrossPaintChangeArea);
                    break;

                case "LocatorChangeSceneAndKeepWalkArea":
                    type = typeof(LocatorChangeSceneAndKeepWalkArea);
                    break;

                case "LocatorChangeSceneAndKeepWalkAreaSdodr":
                    type = typeof(LocatorChangeSceneAndKeepWalkAreaSdodr);
                    break;

                case "LocatorGachiasariClamInitSpawnArea":
                    type = typeof(LocatorGachiasariClamInitSpawnArea);
                    break;

                case "LocatorGachihokoRouteArea":
                    type = typeof(LocatorGachihokoRouteArea);
                    break;

                case "LocatorGoldenIkuraReturnArea":
                    type = typeof(LocatorGoldenIkuraReturnArea);
                    break;

                case "LocatorKumasanRocketChangeStep":
                    type = typeof(LocatorKumasanRocketChangeStep);
                    break;

                case "LocatorLocker":
                    type = typeof(LocatorLocker);
                    break;

                case "LocatorNpcLobby":
                    type = typeof(LocatorNpcLobby);
                    break;

                case "LocatorNpcPlazaFixed":
                    type = typeof(LocatorNpcPlazaFixed);
                    break;

                case "LocatorNpcWorldAgent1":
                    type = typeof(LocatorNpcWorldAgent1);
                    break;

                case "LocatorNpcWorldAgent3":
                    type = typeof(LocatorNpcWorldAgent3);
                    break;

                case "LocatorNpcWorldCommander":
                    type = typeof(LocatorNpcWorldCommander);
                    break;

                case "LocatorNpcWorldWalkRandomAreaSdodr":
                    type = typeof(LocatorNpcWorldWalkRandomAreaSdodr);
                    break;

                case "LocatorPlayerPatchArea":
                    type = typeof(LocatorPlayerPatchArea);
                    break;

                case "LocatorPlayerRejectArea":
                    type = typeof(LocatorPlayerRejectArea);
                    break;

                case "LocatorPlazaJerryFixed":
                    type = typeof(LocatorPlazaJerryFixed);
                    break;

                case "LocatorRandomDigUpPointSpawnArea":
                    type = typeof(LocatorRandomDigUpPointSpawnArea);
                    break;

                case "LocatorSpawner":
                    type = typeof(LocatorSpawner);
                    break;

                case "LocatorTricolMatoiLandingPoint":
                    type = typeof(LocatorTricolMatoiLandingPoint);
                    break;

                case "LocatorTutorial":
                    type = typeof(LocatorTutorial);
                    break;

                case "MissionBossGateway":
                    type = typeof(MissionBossGateway);
                    break;

                case "MissionCheckPoint":
                    type = typeof(MissionCheckPoint);
                    break;

                case "MissionFirstCheckPoint":
                    type = typeof(MissionFirstCheckPoint);
                    break;

                case "MissionGateway":
                    type = typeof(MissionGateway);
                    break;

                case "MissionGatewayChallenge":
                    type = typeof(MissionGatewayChallenge);
                    break;

                case "MissionSeaSurface":
                    type = typeof(MissionSeaSurface);
                    break;

                case "MissionSharkkingWater":
                    type = typeof(MissionSharkkingWater);
                    break;

                case "MovePainterFixedDirection":
                    type = typeof(MovePainterFixedDirection);
                    break;

                case "MovePainterRailFollow":
                    type = typeof(MovePainterRailFollow);
                    break;

                case "Mpt_Fld_DemoCrater":
                    type = typeof(Mpt_Fld_DemoCrater);
                    break;

                case "Mpt_PlayerDead":
                    type = typeof(Mpt_PlayerDead);
                    break;

                case "Mpt_SdodrElevatorNpcWaitFloor":
                    type = typeof(Mpt_SdodrElevatorNpcWaitFloor);
                    break;

                case "NavigateAirBall":
                    type = typeof(NavigateAirBall);
                    break;

                case "NavigateAirBall_Tutorial":
                    type = typeof(NavigateAirBall_Tutorial);
                    break;

                case "NpcIdolDanceA":
                    type = typeof(NpcIdolDanceA);
                    break;

                case "NpcIdolDanceA_Sdodr":
                    type = typeof(NpcIdolDanceA_Sdodr);
                    break;

                case "NpcIdolDanceB":
                    type = typeof(NpcIdolDanceB);
                    break;

                case "NpcIdolDanceB_Sdodr":
                    type = typeof(NpcIdolDanceB_Sdodr);
                    break;

                case "NpcIdolDanceC":
                    type = typeof(NpcIdolDanceC);
                    break;

                case "NpcIdolNewsA":
                    type = typeof(NpcIdolNewsA);
                    break;

                case "NpcIdolNewsA_Sdodr":
                    type = typeof(NpcIdolNewsA_Sdodr);
                    break;

                case "NpcIdolNewsB":
                    type = typeof(NpcIdolNewsB);
                    break;

                case "NpcIdolNewsB_Sdodr":
                    type = typeof(NpcIdolNewsB_Sdodr);
                    break;

                case "NpcIdolNewsC":
                    type = typeof(NpcIdolNewsC);
                    break;

                case "NpcLobbyJerry_CafeCrew_00":
                    type = typeof(NpcLobbyJerry_CafeCrew_00);
                    break;

                case "NpcLobbyJerry_DJ_00":
                    type = typeof(NpcLobbyJerry_DJ_00);
                    break;

                case "NpcMicroOctopus_Sdodr":
                    type = typeof(NpcMicroOctopus_Sdodr);
                    break;

                case "NpcPlazaJerryFest_Dance_00":
                    type = typeof(NpcPlazaJerryFest_Dance_00);
                    break;

                case "NpcPlazaJerry_AssistantDirector_00":
                    type = typeof(NpcPlazaJerry_AssistantDirector_00);
                    break;

                case "NpcPlazaJerry_AssistantDirector_01":
                    type = typeof(NpcPlazaJerry_AssistantDirector_01);
                    break;

                case "NpcPlazaJerry_BuildingCleaning_00":
                    type = typeof(NpcPlazaJerry_BuildingCleaning_00);
                    break;

                case "NpcPlazaJerry_CafeCrew_00":
                    type = typeof(NpcPlazaJerry_CafeCrew_00);
                    break;

                case "NpcPlazaJerry_Director_00":
                    type = typeof(NpcPlazaJerry_Director_00);
                    break;

                case "NpcPlazaJerry_Park":
                    type = typeof(NpcPlazaJerry_Park);
                    break;

                case "NpcPlazaJerry_ParkLongHead":
                    type = typeof(NpcPlazaJerry_ParkLongHead);
                    break;

                case "NpcPlazaJerry_ParkNap":
                    type = typeof(NpcPlazaJerry_ParkNap);
                    break;

                case "NpcPlazaJerry_PlazaCafe_00":
                    type = typeof(NpcPlazaJerry_PlazaCafe_00);
                    break;

                case "NpcPlazaJerry_SdodrSkateboard_00":
                    type = typeof(NpcPlazaJerry_SdodrSkateboard_00);
                    break;

                case "NpcPlazaJerry_SdodrSkateboard_01":
                    type = typeof(NpcPlazaJerry_SdodrSkateboard_01);
                    break;

                case "NpcVsJerry_Float_00":
                    type = typeof(NpcVsJerry_Float_00);
                    break;

                case "NpcVsJerry_Float_02":
                    type = typeof(NpcVsJerry_Float_02);
                    break;

                case "NpcVsJerry_Float_03":
                    type = typeof(NpcVsJerry_Float_03);
                    break;

                case "NpcVsJerry_Float_04":
                    type = typeof(NpcVsJerry_Float_04);
                    break;

                case "NpcVsJerry_HelmetRuins_00":
                    type = typeof(NpcVsJerry_HelmetRuins_00);
                    break;

                case "NpcVsJerry_HelmetRuins_01":
                    type = typeof(NpcVsJerry_HelmetRuins_01);
                    break;

                case "NpcVsJerry_SwimWear_01":
                    type = typeof(NpcVsJerry_SwimWear_01);
                    break;

                case "NpcVsJerry_SwimWear_02":
                    type = typeof(NpcVsJerry_SwimWear_02);
                    break;

                case "NpcVsJerry_SwimWear_07":
                    type = typeof(NpcVsJerry_SwimWear_07);
                    break;

                case "NpcVsJerry_SwimWear_08":
                    type = typeof(NpcVsJerry_SwimWear_08);
                    break;

                case "NpcVsJerry_SwimWear_09":
                    type = typeof(NpcVsJerry_SwimWear_09);
                    break;

                case "NpcVsJerry_SwimWear_Float00":
                    type = typeof(NpcVsJerry_SwimWear_Float00);
                    break;

                case "NpcVsJerry_SwimWear_Float01":
                    type = typeof(NpcVsJerry_SwimWear_Float01);
                    break;

                case "NpcVsJerry_SwimWear_Float02":
                    type = typeof(NpcVsJerry_SwimWear_Float02);
                    break;

                case "NpcVsJerry_SwimWear_Float03":
                    type = typeof(NpcVsJerry_SwimWear_Float03);
                    break;

                case "NpcVsJerry_SwimWear_Float04":
                    type = typeof(NpcVsJerry_SwimWear_Float04);
                    break;

                case "NpcWorldWalkMobASdodr":
                    type = typeof(NpcWorldWalkMobASdodr);
                    break;

                case "NpcWorldWalkMobBSdodr":
                    type = typeof(NpcWorldWalkMobBSdodr);
                    break;

                case "Npc_Commander_Dried":
                    type = typeof(Npc_Commander_Dried);
                    break;

                case "Npc_HeadGearShop":
                    type = typeof(Npc_HeadGearShop);
                    break;

                case "Npc_HeadGearShop_B":
                    type = typeof(Npc_HeadGearShop_B);
                    break;

                case "Npc_HeadGearShop_Sdodr":
                    type = typeof(Npc_HeadGearShop_Sdodr);
                    break;

                case "Npc_JerryFest_AssistantDirector":
                    type = typeof(Npc_JerryFest_AssistantDirector);
                    break;

                case "Npc_JerryFest_Director":
                    type = typeof(Npc_JerryFest_Director);
                    break;

                case "Npc_JerryFest_PlazaCafe":
                    type = typeof(Npc_JerryFest_PlazaCafe);
                    break;

                case "Npc_JerryFest_PollingStation":
                    type = typeof(Npc_JerryFest_PollingStation);
                    break;

                case "Npc_JerryFest_Shop00_00":
                    type = typeof(Npc_JerryFest_Shop00_00);
                    break;

                case "Npc_JerryFest_Shop00_01":
                    type = typeof(Npc_JerryFest_Shop00_01);
                    break;

                case "Npc_JerryFest_Shop00_02":
                    type = typeof(Npc_JerryFest_Shop00_02);
                    break;

                case "Npc_JerryFest_Shop01_00":
                    type = typeof(Npc_JerryFest_Shop01_00);
                    break;

                case "Npc_JerryFest_Shop01_01":
                    type = typeof(Npc_JerryFest_Shop01_01);
                    break;

                case "Npc_JerryFest_Shop01_02":
                    type = typeof(Npc_JerryFest_Shop01_02);
                    break;

                case "Npc_KnickKnacksShop":
                    type = typeof(Npc_KnickKnacksShop);
                    break;

                case "Npc_KnickKnacksShop_Sdodr":
                    type = typeof(Npc_KnickKnacksShop_Sdodr);
                    break;

                case "Npc_Kumasan":
                    type = typeof(Npc_Kumasan);
                    break;

                case "Npc_MasterShark":
                    type = typeof(Npc_MasterShark);
                    break;

                case "Npc_MorayFantom":
                    type = typeof(Npc_MorayFantom);
                    break;

                case "Npc_RailKingSpectacle":
                    type = typeof(Npc_RailKingSpectacle);
                    break;

                case "Npc_SdodrCollector":
                    type = typeof(Npc_SdodrCollector);
                    break;

                case "Npc_SdodrDemoMachineDiva":
                    type = typeof(Npc_SdodrDemoMachineDiva);
                    break;

                case "Npc_SdodrFogOda":
                    type = typeof(Npc_SdodrFogOda);
                    break;

                case "Npc_SdodrHacker":
                    type = typeof(Npc_SdodrHacker);
                    break;

                case "Npc_SdodrHime":
                    type = typeof(Npc_SdodrHime);
                    break;

                case "Npc_SdodrIida":
                    type = typeof(Npc_SdodrIida);
                    break;

                case "Npc_SdodrIida_Odako":
                    type = typeof(Npc_SdodrIida_Odako);
                    break;

                case "Npc_SdodrMizuta":
                    type = typeof(Npc_SdodrMizuta);
                    break;

                case "Npc_SdodrMizuta_Odako":
                    type = typeof(Npc_SdodrMizuta_Odako);
                    break;

                case "Npc_SdodrRestingHime":
                    type = typeof(Npc_SdodrRestingHime);
                    break;

                case "Npc_ShoesShop":
                    type = typeof(Npc_ShoesShop);
                    break;

                case "Npc_ShoesShop_Sdodr":
                    type = typeof(Npc_ShoesShop_Sdodr);
                    break;

                case "Npc_ThinKumasan":
                    type = typeof(Npc_ThinKumasan);
                    break;

                case "Npc_TopsShop":
                    type = typeof(Npc_TopsShop);
                    break;

                case "Npc_TopsShop_Sdodr":
                    type = typeof(Npc_TopsShop_Sdodr);
                    break;

                case "Npc_WeaponShop":
                    type = typeof(Npc_WeaponShop);
                    break;

                case "Npc_WeaponShop_Sdodr":
                    type = typeof(Npc_WeaponShop_Sdodr);
                    break;

                case "Npc_WorldAgent1":
                    type = typeof(Npc_WorldAgent1);
                    break;

                case "Npc_WorldAgent2":
                    type = typeof(Npc_WorldAgent2);
                    break;

                case "Npc_WorldBankaraIdolA":
                    type = typeof(Npc_WorldBankaraIdolA);
                    break;

                case "Npc_WorldBankaraIdolB":
                    type = typeof(Npc_WorldBankaraIdolB);
                    break;

                case "Npc_WorldBankaraIdolC":
                    type = typeof(Npc_WorldBankaraIdolC);
                    break;

                case "Obj_AssemblyTool":
                    type = typeof(Obj_AssemblyTool);
                    break;

                case "Obj_D2-02KebaInk":
                    type = typeof(Obj_D2_02KebaInk);
                    break;

                case "Obj_D2-02KebaInkCut3":
                    type = typeof(Obj_D2_02KebaInkCut3);
                    break;

                case "Obj_D2-02KebaInkCut3Tentacle":
                    type = typeof(Obj_D2_02KebaInkCut3Tentacle);
                    break;

                case "Obj_D2-02KebaInkCut3Wall":
                    type = typeof(Obj_D2_02KebaInkCut3Wall);
                    break;

                case "Obj_D2-02KebaInkCut3WallB":
                    type = typeof(Obj_D2_02KebaInkCut3WallB);
                    break;

                case "Obj_D6_InkPillar":
                    type = typeof(Obj_D6_InkPillar);
                    break;

                case "Obj_Demo_EventCameraScreen":
                    type = typeof(Obj_Demo_EventCameraScreen);
                    break;

                case "Obj_Demo_MsnClt302":
                    type = typeof(Obj_Demo_MsnClt302);
                    break;

                case "Obj_Demo_MsnHed302":
                    type = typeof(Obj_Demo_MsnHed302);
                    break;

                case "Obj_Demo_MsnHed305":
                    type = typeof(Obj_Demo_MsnHed305);
                    break;

                case "Obj_Demo_MsnShs302":
                    type = typeof(Obj_Demo_MsnShs302);
                    break;

                case "Obj_Demo_MsnShs302_R":
                    type = typeof(Obj_Demo_MsnShs302_R);
                    break;

                case "Obj_Demo_Newspaper":
                    type = typeof(Obj_Demo_Newspaper);
                    break;

                case "Obj_Demo_ShaverEngine":
                    type = typeof(Obj_Demo_ShaverEngine);
                    break;

                case "Obj_FoldingFan_BankaraIdolA":
                    type = typeof(Obj_FoldingFan_BankaraIdolA);
                    break;

                case "Obj_HighLiftLoaderUnder00":
                    type = typeof(Obj_HighLiftLoaderUnder00);
                    break;

                case "Obj_HighLiftLoaderUnder04":
                    type = typeof(Obj_HighLiftLoaderUnder04);
                    break;

                case "Obj_LobbyProjector":
                    type = typeof(Obj_LobbyProjector);
                    break;

                case "Obj_Mask_BankaraIdolA":
                    type = typeof(Obj_Mask_BankaraIdolA);
                    break;

                case "Obj_Mask_BankaraIdolB":
                    type = typeof(Obj_Mask_BankaraIdolB);
                    break;

                case "Obj_Mask_BankaraIdolC":
                    type = typeof(Obj_Mask_BankaraIdolC);
                    break;

                case "Obj_MsnWallaBox15x8x15":
                    type = typeof(Obj_MsnWallaBox15x8x15);
                    break;

                case "Obj_MsnWallaCylinder10x10x10":
                    type = typeof(Obj_MsnWallaCylinder10x10x10);
                    break;

                case "Obj_RivalDroneDemoSdodr":
                    type = typeof(Obj_RivalDroneDemoSdodr);
                    break;

                case "Obj_RollingCone":
                    type = typeof(Obj_RollingCone);
                    break;

                case "Obj_Ruins03PatrolLamp":
                    type = typeof(Obj_Ruins03PatrolLamp);
                    break;

                case "Obj_Ruins03WinchSetL":
                    type = typeof(Obj_Ruins03WinchSetL);
                    break;

                case "Obj_Ruins03WinchSetR":
                    type = typeof(Obj_Ruins03WinchSetR);
                    break;

                case "Obj_SdodrBackScreen":
                    type = typeof(Obj_SdodrBackScreen);
                    break;

                case "Obj_SdodrColorChip":
                    type = typeof(Obj_SdodrColorChip);
                    break;

                case "Obj_SdodrColorPalette":
                    type = typeof(Obj_SdodrColorPalette);
                    break;

                case "Obj_SdodrDemo_LogoutElevator":
                    type = typeof(Obj_SdodrDemo_LogoutElevator);
                    break;

                case "Obj_SdodrMidiLinkPolyhedron":
                    type = typeof(Obj_SdodrMidiLinkPolyhedron);
                    break;

                case "Obj_SdodrMovieScreen":
                    type = typeof(Obj_SdodrMovieScreen);
                    break;

                case "Obj_SdodrOdakoSwingLeg0":
                    type = typeof(Obj_SdodrOdakoSwingLeg0);
                    break;

                case "Obj_SdodrOdakoSwingLeg1":
                    type = typeof(Obj_SdodrOdakoSwingLeg1);
                    break;

                case "Obj_SdodrOdakoSwingLeg2":
                    type = typeof(Obj_SdodrOdakoSwingLeg2);
                    break;

                case "Obj_SdodrOdakoSwingLeg3":
                    type = typeof(Obj_SdodrOdakoSwingLeg3);
                    break;

                case "Obj_SdodrOdakoSwingLegEX0":
                    type = typeof(Obj_SdodrOdakoSwingLegEX0);
                    break;

                case "Obj_SdodrOdakoSwingLegEX1":
                    type = typeof(Obj_SdodrOdakoSwingLegEX1);
                    break;

                case "Obj_SdodrOdakoSwingLegEX2":
                    type = typeof(Obj_SdodrOdakoSwingLegEX2);
                    break;

                case "Obj_SdodrOdakoSwingLegEX3":
                    type = typeof(Obj_SdodrOdakoSwingLegEX3);
                    break;

                case "Obj_SdodrOdakoSwingLegStoryBoss0":
                    type = typeof(Obj_SdodrOdakoSwingLegStoryBoss0);
                    break;

                case "Obj_SdodrOdakoSwingLegStoryBoss1":
                    type = typeof(Obj_SdodrOdakoSwingLegStoryBoss1);
                    break;

                case "Obj_SdodrOdakoSwingLegStoryBoss2":
                    type = typeof(Obj_SdodrOdakoSwingLegStoryBoss2);
                    break;

                case "Obj_SdodrOdakoSwingLegStoryBoss3":
                    type = typeof(Obj_SdodrOdakoSwingLegStoryBoss3);
                    break;

                case "Obj_SdodrPlayerBaggage":
                    type = typeof(Obj_SdodrPlayerBaggage);
                    break;

                case "Obj_SdodrPlayerMakeTrainInside":
                    type = typeof(Obj_SdodrPlayerMakeTrainInside);
                    break;

                case "Obj_SdodrPlazaSofa":
                    type = typeof(Obj_SdodrPlazaSofa);
                    break;

                case "Obj_SdodrTentaclesB_Laptop":
                    type = typeof(Obj_SdodrTentaclesB_Laptop);
                    break;

                case "Obj_SdodrTrainInsideBack":
                    type = typeof(Obj_SdodrTrainInsideBack);
                    break;

                case "Obj_Shakeship00Gear":
                    type = typeof(Obj_Shakeship00Gear);
                    break;

                case "Obj_Shakeship00PatrolLamp":
                    type = typeof(Obj_Shakeship00PatrolLamp);
                    break;

                case "Obj_SpaceDemo":
                    type = typeof(Obj_SpaceDemo);
                    break;

                case "Obj_StaffRollName":
                    type = typeof(Obj_StaffRollName);
                    break;

                case "Obj_TicketGate":
                    type = typeof(Obj_TicketGate);
                    break;

                case "Obj_Tumbleweed":
                    type = typeof(Obj_Tumbleweed);
                    break;

                case "Obj_Whistle_BankaraIdolB":
                    type = typeof(Obj_Whistle_BankaraIdolB);
                    break;

                case "Obj_YagaraBox00":
                    type = typeof(Obj_YagaraBox00);
                    break;

                case "Obj_YagaraBox01":
                    type = typeof(Obj_YagaraBox01);
                    break;

                case "Obj_YagaraBox02":
                    type = typeof(Obj_YagaraBox02);
                    break;

                case "Obj_YagaraBox03":
                    type = typeof(Obj_YagaraBox03);
                    break;

                case "Obj_YagaraBox04":
                    type = typeof(Obj_YagaraBox04);
                    break;

                case "Obj_YagaraBox05":
                    type = typeof(Obj_YagaraBox05);
                    break;

                case "OneShotMissDieTag":
                    type = typeof(OneShotMissDieTag);
                    break;

                case "PaintedArea_Cube":
                    type = typeof(PaintedArea_Cube);
                    break;

                case "PaintedArea_Cylinder":
                    type = typeof(PaintedArea_Cylinder);
                    break;

                case "PaintingLiftSlide12M":
                    type = typeof(PaintingLiftSlide12M);
                    break;

                case "PaintingLiftSlide6M":
                    type = typeof(PaintingLiftSlide6M);
                    break;

                case "PaintTargetAreaManagerSdodr":
                    type = typeof(PaintTargetAreaManagerSdodr);
                    break;

                case "PaintTargetArea_Cube":
                    type = typeof(PaintTargetArea_Cube);
                    break;

                case "PaintTargetArea_Cylinder":
                    type = typeof(PaintTargetArea_Cylinder);
                    break;

                case "Periscope":
                    type = typeof(Periscope);
                    break;

                case "Pipeline":
                    type = typeof(Pipeline);
                    break;

                case "Propeller":
                    type = typeof(Propeller);
                    break;

                case "PropellerOnline2":
                    type = typeof(PropellerOnline2);
                    break;

                case "PropellerOnline2PopOut":
                    type = typeof(PropellerOnline2PopOut);
                    break;

                case "PropellerOnlineWithLift":
                    type = typeof(PropellerOnlineWithLift);
                    break;

                case "PropellerUp":
                    type = typeof(PropellerUp);
                    break;

                case "PropellerUpSdodr":
                    type = typeof(PropellerUpSdodr);
                    break;

                case "RailingObj":
                    type = typeof(RailingObj);
                    break;

                case "RailKingPilotSpectacle":
                    type = typeof(RailKingPilotSpectacle);
                    break;

                case "RestartPosUpdateArea":
                    type = typeof(RestartPosUpdateArea);
                    break;

                case "RivalAppearPoint":
                    type = typeof(RivalAppearPoint);
                    break;

                case "RivalAppearSequencerSdodr":
                    type = typeof(RivalAppearSequencerSdodr);
                    break;

                case "RL":
                    type = typeof(RL);
                    break;

                case "RocketSpectacle":
                    type = typeof(RocketSpectacle);
                    break;

                case "RR":
                    type = typeof(RR);
                    break;

                case "SalmonBuddySpectacle":
                    type = typeof(SalmonBuddySpectacle);
                    break;

                case "Sdodr_Entrance":
                    type = typeof(Sdodr_Entrance);
                    break;

                case "ShootingAirBall":
                    type = typeof(ShootingAirBall);
                    break;

                case "ShootingBoxL":
                    type = typeof(ShootingBoxL);
                    break;

                case "ShootingBoxLHard":
                    type = typeof(ShootingBoxLHard);
                    break;

                case "ShootingBoxLHard_EnemyBreakable":
                    type = typeof(ShootingBoxLHard_EnemyBreakable);
                    break;

                case "ShootingBoxL_EnemyBreakable":
                    type = typeof(ShootingBoxL_EnemyBreakable);
                    break;

                case "ShootingBoxS":
                    type = typeof(ShootingBoxS);
                    break;

                case "ShootingBoxS_EnemyBreakable":
                    type = typeof(ShootingBoxS_EnemyBreakable);
                    break;

                case "ShootingNGAirBall":
                    type = typeof(ShootingNGAirBall);
                    break;

                case "SighterTarget":
                    type = typeof(SighterTarget);
                    break;

                case "SighterTargetSdodr":
                    type = typeof(SighterTargetSdodr);
                    break;

                case "SighterTarget_Coop_Bomber":
                    type = typeof(SighterTarget_Coop_Bomber);
                    break;

                case "SighterTarget_Coop_Large":
                    type = typeof(SighterTarget_Coop_Large);
                    break;

                case "SighterTarget_Coop_Shield":
                    type = typeof(SighterTarget_Coop_Shield);
                    break;

                case "SighterTarget_Coop_Small":
                    type = typeof(SighterTarget_Coop_Small);
                    break;

                case "SighterTarget_Coop_Standard":
                    type = typeof(SighterTarget_Coop_Standard);
                    break;

                case "SighterTarget_Coop_Standard_Move":
                    type = typeof(SighterTarget_Coop_Standard_Move);
                    break;

                case "SighterTarget_Large":
                    type = typeof(SighterTarget_Large);
                    break;

                case "SighterTarget_Move":
                    type = typeof(SighterTarget_Move);
                    break;

                case "SighterTarget_TipsTrial":
                    type = typeof(SighterTarget_TipsTrial);
                    break;

                case "SighterTarget_TipsTrialMove":
                    type = typeof(SighterTarget_TipsTrialMove);
                    break;

                case "SnakeBlock":
                    type = typeof(SnakeBlock);
                    break;

                case "SnakeBlockSdodr":
                    type = typeof(SnakeBlockSdodr);
                    break;

                case "SoundDryControlArea":
                    type = typeof(SoundDryControlArea);
                    break;

                case "SoundFxArea":
                    type = typeof(SoundFxArea);
                    break;

                case "SoundShape_Box":
                    type = typeof(SoundShape_Box);
                    break;

                case "SoundShape_Cylinder":
                    type = typeof(SoundShape_Cylinder);
                    break;

                case "SoundShape_Point":
                    type = typeof(SoundShape_Point);
                    break;

                case "SoundShape_Sphere":
                    type = typeof(SoundShape_Sphere);
                    break;

                case "SpawnerForBeaconGimmick_TipsTrial":
                    type = typeof(SpawnerForBeaconGimmick_TipsTrial);
                    break;

                case "SpawnerForShieldGimmick_TipsTrial":
                    type = typeof(SpawnerForShieldGimmick_TipsTrial);
                    break;

                case "SpawnerForSprinklerGimmick":
                    type = typeof(SpawnerForSprinklerGimmick);
                    break;

                case "SpawnerForSprinklerGimmick_TipsTrial":
                    type = typeof(SpawnerForSprinklerGimmick_TipsTrial);
                    break;

                case "SpectacleCoreBattleManager":
                    type = typeof(SpectacleCoreBattleManager);
                    break;

                case "SpectacleManager":
                    type = typeof(SpectacleManager);
                    break;

                case "SplAutoWarpObj":
                    type = typeof(SplAutoWarpObj);
                    break;

                case "SplAutoWarpPoint":
                    type = typeof(SplAutoWarpPoint);
                    break;

                case "SplAutoWarpPointWithChangeScene":
                    type = typeof(SplAutoWarpPointWithChangeScene);
                    break;

                case "SplConcreteSpawnerSdodr":
                    type = typeof(SplConcreteSpawnerSdodr);
                    break;

                case "SplEnemyAlertArea":
                    type = typeof(SplEnemyAlertArea);
                    break;

                case "SplEnemyBallKingSdodr":
                    type = typeof(SplEnemyBallKingSdodr);
                    break;

                case "SplEnemyBallKingSdodrStrong":
                    type = typeof(SplEnemyBallKingSdodrStrong);
                    break;

                case "SplEnemyBarrierKingSdodr":
                    type = typeof(SplEnemyBarrierKingSdodr);
                    break;

                case "SplEnemyBarrierKingSdodrOdako":
                    type = typeof(SplEnemyBarrierKingSdodrOdako);
                    break;

                case "SplEnemyBarrierKingSdodrOdakoEX":
                    type = typeof(SplEnemyBarrierKingSdodrOdakoEX);
                    break;

                case "SplEnemyBarrierKingSdodrOdakoTentacleDevice":
                    type = typeof(SplEnemyBarrierKingSdodrOdakoTentacleDevice);
                    break;

                case "SplEnemyBarrierKingSdodrStoryBoss":
                    type = typeof(SplEnemyBarrierKingSdodrStoryBoss);
                    break;

                case "SplEnemyBombBlowSdodr":
                    type = typeof(SplEnemyBombBlowSdodr);
                    break;

                case "SplEnemyBombBlowSdodr_PoisonMist":
                    type = typeof(SplEnemyBombBlowSdodr_PoisonMist);
                    break;

                case "SplEnemyBombBlowSdodr_RandomBomb":
                    type = typeof(SplEnemyBombBlowSdodr_RandomBomb);
                    break;

                case "SplEnemyCharge":
                    type = typeof(SplEnemyCharge);
                    break;

                case "SplEnemyChargeElite":
                    type = typeof(SplEnemyChargeElite);
                    break;

                case "SplEnemyHopperSdodr":
                    type = typeof(SplEnemyHopperSdodr);
                    break;

                case "SplEnemyRailKing":
                    type = typeof(SplEnemyRailKing);
                    break;

                case "SplEnemyShellSdodr":
                    type = typeof(SplEnemyShellSdodr);
                    break;

                case "SplEnemyShellSdodr_Shell":
                    type = typeof(SplEnemyShellSdodr_Shell);
                    break;

                case "SplEnemySpinner":
                    type = typeof(SplEnemySpinner);
                    break;

                case "SplEnemySprinklerSdodr":
                    type = typeof(SplEnemySprinklerSdodr);
                    break;

                case "SplEnemyTowerKingSdodr":
                    type = typeof(SplEnemyTowerKingSdodr);
                    break;

                case "SplEnemyTowerKingSdodrStrong":
                    type = typeof(SplEnemyTowerKingSdodrStrong);
                    break;

                case "SplEnemyTreeSdodr":
                    type = typeof(SplEnemyTreeSdodr);
                    break;

                case "SplEnemyZakoLargeSdodr":
                    type = typeof(SplEnemyZakoLargeSdodr);
                    break;

                case "SplEnemyZakoLargeSdodrMountHopper":
                    type = typeof(SplEnemyZakoLargeSdodrMountHopper);
                    break;

                case "SplEnemyZakoLargeSdodrMountShell":
                    type = typeof(SplEnemyZakoLargeSdodrMountShell);
                    break;

                case "SplEnemyZakoLargeSdodrMountSprinkler":
                    type = typeof(SplEnemyZakoLargeSdodrMountSprinkler);
                    break;

                case "SplEnemyZakoLargeSdodr_Tutorial":
                    type = typeof(SplEnemyZakoLargeSdodr_Tutorial);
                    break;

                case "SplEnemyZakoSmallSdodr":
                    type = typeof(SplEnemyZakoSmallSdodr);
                    break;

                case "SplEnemyZakoSmallSdodr_FixedChain":
                    type = typeof(SplEnemyZakoSmallSdodr_FixedChain);
                    break;

                case "SplEnemyZakoSmallSdodr_Tutorial":
                    type = typeof(SplEnemyZakoSmallSdodr_Tutorial);
                    break;

                case "SplEnemyZakoStandardSdodr":
                    type = typeof(SplEnemyZakoStandardSdodr);
                    break;

                case "SplEnemyZakoStandardSdodr_Tutorial":
                    type = typeof(SplEnemyZakoStandardSdodr_Tutorial);
                    break;

                case "SplEntryLiftSdodr":
                    type = typeof(SplEntryLiftSdodr);
                    break;

                case "SplExplodeSpawnerSdodr":
                    type = typeof(SplExplodeSpawnerSdodr);
                    break;

                case "SplExplodeSpawnerSdodr_SuperHard":
                    type = typeof(SplExplodeSpawnerSdodr_SuperHard);
                    break;

                case "SplFieldEnvEffectArea":
                    type = typeof(SplFieldEnvEffectArea);
                    break;

                case "SplFieldEnvEffectLocator":
                    type = typeof(SplFieldEnvEffectLocator);
                    break;

                case "SplLobbyCoopDirector":
                    type = typeof(SplLobbyCoopDirector);
                    break;

                case "SplMetaSpawnerSdodr":
                    type = typeof(SplMetaSpawnerSdodr);
                    break;

                case "SplMissionBigWorldTreasureA":
                    type = typeof(SplMissionBigWorldTreasureA);
                    break;

                case "SplMissionBigWorldTreasureB":
                    type = typeof(SplMissionBigWorldTreasureB);
                    break;

                case "SplMissionBigWorldTreasureC":
                    type = typeof(SplMissionBigWorldTreasureC);
                    break;

                case "SplMissionSalmonBuddy":
                    type = typeof(SplMissionSalmonBuddy);
                    break;

                case "SplMissionSalmonBuddyLeadPlayerArea":
                    type = typeof(SplMissionSalmonBuddyLeadPlayerArea);
                    break;

                case "SplMissionStageDummyTreasureA":
                    type = typeof(SplMissionStageDummyTreasureA);
                    break;

                case "SplMissionStageDummyTreasureB":
                    type = typeof(SplMissionStageDummyTreasureB);
                    break;

                case "SplMissionStageDummyTreasureC":
                    type = typeof(SplMissionStageDummyTreasureC);
                    break;

                case "SplMissionStageTreasureA":
                    type = typeof(SplMissionStageTreasureA);
                    break;

                case "SplMissionStageTreasureB":
                    type = typeof(SplMissionStageTreasureB);
                    break;

                case "SplMissionStageTreasureC":
                    type = typeof(SplMissionStageTreasureC);
                    break;

                case "SplPlayerMediationForLogic":
                    type = typeof(SplPlayerMediationForLogic);
                    break;

                case "SplPlazaSalmonBuddy":
                    type = typeof(SplPlazaSalmonBuddy);
                    break;

                case "SplPlazaSalmonBuddyLocator":
                    type = typeof(SplPlazaSalmonBuddyLocator);
                    break;

                case "SplRailEffectObj":
                    type = typeof(SplRailEffectObj);
                    break;

                case "SplRailPaintObj":
                    type = typeof(SplRailPaintObj);
                    break;

                case "SplRandomDeactivator":
                    type = typeof(SplRandomDeactivator);
                    break;

                case "SplRivalMetaSpawnerSdodr":
                    type = typeof(SplRivalMetaSpawnerSdodr);
                    break;

                case "SplSdodrRestingHimeLocator":
                    type = typeof(SplSdodrRestingHimeLocator);
                    break;

                case "SplSdodrSalmonBuddy":
                    type = typeof(SplSdodrSalmonBuddy);
                    break;

                case "SplSwitchComboArea":
                    type = typeof(SplSwitchComboArea);
                    break;

                case "SplTutorialDirector":
                    type = typeof(SplTutorialDirector);
                    break;

                case "SplWorldLotteryDroneObj":
                    type = typeof(SplWorldLotteryDroneObj);
                    break;

                case "Sponge":
                    type = typeof(Sponge);
                    break;

                case "SpongeRectangle":
                    type = typeof(SpongeRectangle);
                    break;

                case "SpongeSdodr":
                    type = typeof(SpongeSdodr);
                    break;

                case "SpongeSmall":
                    type = typeof(SpongeSmall);
                    break;

                case "SpongeSmall_3p0_VS":
                    type = typeof(SpongeSmall_3p0_VS);
                    break;

                case "SpongeSmall_VS":
                    type = typeof(SpongeSmall_VS);
                    break;

                case "SpongeTall":
                    type = typeof(SpongeTall);
                    break;

                case "SpongeTall_3p5_VS":
                    type = typeof(SpongeTall_3p5_VS);
                    break;

                case "SpongeTall_4p0_VS":
                    type = typeof(SpongeTall_4p0_VS);
                    break;

                case "SpongeTall_4p5_VS":
                    type = typeof(SpongeTall_4p5_VS);
                    break;

                case "SpongeTall_6p0_VS":
                    type = typeof(SpongeTall_6p0_VS);
                    break;

                case "StartPos":
                    type = typeof(StartPos);
                    break;

                case "StartPosForBigWorld":
                    type = typeof(StartPosForBigWorld);
                    break;

                case "StartPosForCoopBossRound":
                    type = typeof(StartPosForCoopBossRound);
                    break;

                case "StartPosForLaunchPadWorld":
                    type = typeof(StartPosForLaunchPadWorld);
                    break;

                case "StartPosForSdodrElevator":
                    type = typeof(StartPosForSdodrElevator);
                    break;

                case "StartPosForSdodrWorld":
                    type = typeof(StartPosForSdodrWorld);
                    break;

                case "StartPosTipsTrial":
                    type = typeof(StartPosTipsTrial);
                    break;

                case "SwitchPaint":
                    type = typeof(SwitchPaint);
                    break;

                case "SwitchPaintSmall":
                    type = typeof(SwitchPaintSmall);
                    break;

                case "SwitchPaintSmallEasy":
                    type = typeof(SwitchPaintSmallEasy);
                    break;

                case "SwitchShock":
                    type = typeof(SwitchShock);
                    break;

                case "SwitchStep":
                    type = typeof(SwitchStep);
                    break;

                case "TreasureRippleEmitObj":
                    type = typeof(TreasureRippleEmitObj);
                    break;

                case "UtuboLight":
                    type = typeof(UtuboLight);
                    break;

                case "VehicleSpectacle":
                    type = typeof(VehicleSpectacle);
                    break;

                case "VictoryLiftSdodr":
                    type = typeof(VictoryLiftSdodr);
                    break;

                case "WoodenBox":
                    type = typeof(WoodenBox);
                    break;

                case "WoodenBoxHard":
                    type = typeof(WoodenBoxHard);
                    break;

                case "WoodenBoxHardSdodr":
                    type = typeof(WoodenBoxHardSdodr);
                    break;

                case "WoodenBoxHard_NG":
                    type = typeof(WoodenBoxHard_NG);
                    break;

                case "WoodenBoxSdodr":
                    type = typeof(WoodenBoxSdodr);
                    break;

                case "WoodenBoxSmall":
                    type = typeof(WoodenBoxSmall);
                    break;

                case "WoodenBoxSmallSdodr":
                    type = typeof(WoodenBoxSmallSdodr);
                    break;

                case "WoodenBox_NG":
                    type = typeof(WoodenBox_NG);
                    break;

                case "WorldDigUpPoint":
                    type = typeof(WorldDigUpPoint);
                    break;

                case "WorldLocationArea":
                    type = typeof(WorldLocationArea);
                    break;

                case "チャージャータワースポナー":
                    type = typeof(チャージャータワースポナー);
                    break;

                case "ノコノコスポナー":
                    type = typeof(ノコノコスポナー);
                    break;

                case "ピンポンツリースポナー":
                    type = typeof(ピンポンツリースポナー);
                    break;

                case "ボムピューピュ－スポナー":
                    type = typeof(ボムピューピュ_スポナー);
                    break;

                case "跳ねる敵スポナー":
                    type = typeof(跳ねる敵スポナー);
                    break;

                case "雑魚中スポナー":
                    type = typeof(雑魚中スポナー);
                    break;

                case "雑魚大スポナー":
                    type = typeof(雑魚大スポナー);
                    break;

                case "雑魚小スポナー":
                    type = typeof(雑魚小スポナー);
                    break;

                case "DObj_DowneyCarpetSAND":
                    type = typeof(DObj_DowneyCarpetSAND);
                    break;

                case "DObj_FldObj_BigRunManbou00Monitor":
                    type = typeof(DObj_FldObj_BigRunManbou00Monitor);
                    break;

                case "DObj_FldObj_InkSprinklerSAND":
                    type = typeof(DObj_FldObj_InkSprinklerSAND);
                    break;

                case "DObj_FldObj_Manbou00Flag":
                    type = typeof(DObj_FldObj_Manbou00Flag);
                    break;

                case "DObj_FldObj_Manbou00MonitorBG":
                    type = typeof(DObj_FldObj_Manbou00MonitorBG);
                    break;

                case "DObj_FldObj_PlazaSANDBigStageRoll":
                    type = typeof(DObj_FldObj_PlazaSANDBigStageRoll);
                    break;

                case "DObj_FldObj_PlazaSANDBigStage_FestFirstHalfIdolStage":
                    type = typeof(DObj_FldObj_PlazaSANDBigStage_FestFirstHalfIdolStage);
                    break;

                case "DObj_FldObj_PlazaSANDBigStage_Fest_WaitingForResultIdolStage":
                    type = typeof(DObj_FldObj_PlazaSANDBigStage_Fest_WaitingForResultIdolStage);
                    break;

                case "DObj_Fld_PlazaSANDLandMark":
                    type = typeof(DObj_Fld_PlazaSANDLandMark);
                    break;

                case "DObj_Fld_PlazaSANDLandMark_FestSecondHalf":
                    type = typeof(DObj_Fld_PlazaSANDLandMark_FestSecondHalf);
                    break;

                case "DObj_Fld_PlazaSANDLandMark_FestWaitingForResult":
                    type = typeof(DObj_Fld_PlazaSANDLandMark_FestWaitingForResult);
                    break;

                case "DObj_PlazaSANDLight":
                    type = typeof(DObj_PlazaSANDLight);
                    break;

                case "DObj_SpikyChairSAND":
                    type = typeof(DObj_SpikyChairSAND);
                    break;

                case "DuckingAreaSAND":
                    type = typeof(DuckingAreaSAND);
                    break;

                case "Fld_BigRunManbou00":
                    type = typeof(Fld_BigRunManbou00);
                    break;

                case "Fld_Manbou00":
                    type = typeof(Fld_Manbou00);
                    break;

                case "Fld_PlazaSAND":
                    type = typeof(Fld_PlazaSAND);
                    break;

                case "Lft_FldObj_BigRunManbou00Stage":
                    type = typeof(Lft_FldObj_BigRunManbou00Stage);
                    break;

                case "Lft_FldObj_GrandFestBGSANDManbou00":
                    type = typeof(Lft_FldObj_GrandFestBGSANDManbou00);
                    break;

                case "Lft_FldObj_Joheki03_PackTcl":
                    type = typeof(Lft_FldObj_Joheki03_PackTcl);
                    break;

                case "Lft_FldObj_Manbou00IdolStage":
                    type = typeof(Lft_FldObj_Manbou00IdolStage);
                    break;

                case "Lft_FldObj_Manbou00Lift":
                    type = typeof(Lft_FldObj_Manbou00Lift);
                    break;

                case "Lft_FldObj_Manbou00Stage":
                    type = typeof(Lft_FldObj_Manbou00Stage);
                    break;

                case "Lft_FldObj_PlazaClosingSAND":
                    type = typeof(Lft_FldObj_PlazaClosingSAND);
                    break;

                case "Lft_FldObj_PlazaSANDMediumStage":
                    type = typeof(Lft_FldObj_PlazaSANDMediumStage);
                    break;

                case "Lft_FldObj_PlazaSANDMediumStageNight":
                    type = typeof(Lft_FldObj_PlazaSANDMediumStageNight);
                    break;

                case "Lft_FldObj_PlazaSANDSmallStage":
                    type = typeof(Lft_FldObj_PlazaSANDSmallStage);
                    break;

                case "Lft_FldObj_PlazaSANDSmallStageNight":
                    type = typeof(Lft_FldObj_PlazaSANDSmallStageNight);
                    break;

                case "Lft_FldObj_SurimiStuidoBG_SAND":
                    type = typeof(Lft_FldObj_SurimiStuidoBG_SAND);
                    break;

                case "Lft_Fld_GrandFestBGSAND":
                    type = typeof(Lft_Fld_GrandFestBGSAND);
                    break;

                case "Lft_Fld_PlazaSANDIdolStalls":
                    type = typeof(Lft_Fld_PlazaSANDIdolStalls);
                    break;

                case "LocatorJerryCrowdArea":
                    type = typeof(LocatorJerryCrowdArea);
                    break;

                case "LocatorMassModelAreaLectSAND":
                    type = typeof(LocatorMassModelAreaLectSAND);
                    break;

                case "LocatorMassModelExclusiveAreaLectSAND":
                    type = typeof(LocatorMassModelExclusiveAreaLectSAND);
                    break;

                case "LocatorMassModelSingleSAND":
                    type = typeof(LocatorMassModelSingleSAND);
                    break;

                case "LocatorPlayerForceStainArea":
                    type = typeof(LocatorPlayerForceStainArea);
                    break;

                case "LocatorTricolTenjinSpawnerLandingPoint":
                    type = typeof(LocatorTricolTenjinSpawnerLandingPoint);
                    break;

                case "NpcIdolDanceA-SAND-NormalWear":
                    type = typeof(NpcIdolDanceA_SAND_NormalWear);
                    break;

                case "NpcIdolDanceA-SAND-VS":
                    type = typeof(NpcIdolDanceA_SAND_VS);
                    break;

                case "NpcIdolDanceA-SAND":
                    type = typeof(NpcIdolDanceA_SAND);
                    break;

                case "NpcIdolDanceA_Fsodr-SAND-NormalWear":
                    type = typeof(NpcIdolDanceA_Fsodr_SAND_NormalWear);
                    break;

                case "NpcIdolDanceA_Fsodr-SAND-VS":
                    type = typeof(NpcIdolDanceA_Fsodr_SAND_VS);
                    break;

                case "NpcIdolDanceA_Fsodr-SAND":
                    type = typeof(NpcIdolDanceA_Fsodr_SAND);
                    break;

                case "NpcIdolDanceA_Sdodr-SAND-NormalWear":
                    type = typeof(NpcIdolDanceA_Sdodr_SAND_NormalWear);
                    break;

                case "NpcIdolDanceA_Sdodr-SAND-VS":
                    type = typeof(NpcIdolDanceA_Sdodr_SAND_VS);
                    break;

                case "NpcIdolDanceA_Sdodr-SAND":
                    type = typeof(NpcIdolDanceA_Sdodr_SAND);
                    break;

                case "NpcIdolDanceB-SAND-NormalWear":
                    type = typeof(NpcIdolDanceB_SAND_NormalWear);
                    break;

                case "NpcIdolDanceB-SAND-VS":
                    type = typeof(NpcIdolDanceB_SAND_VS);
                    break;

                case "NpcIdolDanceB-SAND":
                    type = typeof(NpcIdolDanceB_SAND);
                    break;

                case "NpcIdolDanceB_Fsodr-SAND-NormalWear":
                    type = typeof(NpcIdolDanceB_Fsodr_SAND_NormalWear);
                    break;

                case "NpcIdolDanceB_Fsodr-SAND-VS":
                    type = typeof(NpcIdolDanceB_Fsodr_SAND_VS);
                    break;

                case "NpcIdolDanceB_Fsodr-SAND":
                    type = typeof(NpcIdolDanceB_Fsodr_SAND);
                    break;

                case "NpcIdolDanceB_Sdodr-SAND-NormalWear":
                    type = typeof(NpcIdolDanceB_Sdodr_SAND_NormalWear);
                    break;

                case "NpcIdolDanceB_Sdodr-SAND-VS":
                    type = typeof(NpcIdolDanceB_Sdodr_SAND_VS);
                    break;

                case "NpcIdolDanceB_Sdodr-SAND":
                    type = typeof(NpcIdolDanceB_Sdodr_SAND);
                    break;

                case "NpcIdolDanceC-SAND-NormalWear":
                    type = typeof(NpcIdolDanceC_SAND_NormalWear);
                    break;

                case "NpcIdolDanceC-SAND-VS":
                    type = typeof(NpcIdolDanceC_SAND_VS);
                    break;

                case "NpcIdolDanceC-SAND":
                    type = typeof(NpcIdolDanceC_SAND);
                    break;

                case "NpcPlazaFesJerry_Child_DObj_SAND":
                    type = typeof(NpcPlazaFesJerry_Child_DObj_SAND);
                    break;

                case "NpcPlazaFesJerry_ShopVisitorChild_DObj_SAND":
                    type = typeof(NpcPlazaFesJerry_ShopVisitorChild_DObj_SAND);
                    break;

                case "NpcPlazaJerry_CafeVisitorChild_DObj_SAND":
                    type = typeof(NpcPlazaJerry_CafeVisitorChild_DObj_SAND);
                    break;

                case "NpcPlazaJerry_CafeVisitor_DObj_SAND":
                    type = typeof(NpcPlazaJerry_CafeVisitor_DObj_SAND);
                    break;

                case "NpcPlazaJerry_CafeVisitor_DObj_SAND_Cstage":
                    type = typeof(NpcPlazaJerry_CafeVisitor_DObj_SAND_Cstage);
                    break;

                case "NpcPlazaJerry_SdodrNap_DObj_SAND":
                    type = typeof(NpcPlazaJerry_SdodrNap_DObj_SAND);
                    break;

                case "NpcPlazaJerry_SdodrSkateboard_00_DObj_SAND":
                    type = typeof(NpcPlazaJerry_SdodrSkateboard_00_DObj_SAND);
                    break;

                case "NpcTakowasa_SAND":
                    type = typeof(NpcTakowasa_SAND);
                    break;

                case "NpcVsJerry_Carousel01_00_DObj_SAND":
                    type = typeof(NpcVsJerry_Carousel01_00_DObj_SAND);
                    break;

                case "NpcVsJerry_Carousel02_01_DObj_SAND":
                    type = typeof(NpcVsJerry_Carousel02_01_DObj_SAND);
                    break;

                case "NpcVsJerry_Foodshop00_00_DObj_SAND":
                    type = typeof(NpcVsJerry_Foodshop00_00_DObj_SAND);
                    break;

                case "NpcVsJerry_GeneralLow_04_DObj_SAND":
                    type = typeof(NpcVsJerry_GeneralLow_04_DObj_SAND);
                    break;

                case "NpcVsJerry_GeneralLow_04_DObj_SAND_01":
                    type = typeof(NpcVsJerry_GeneralLow_04_DObj_SAND_01);
                    break;

                case "NpcVsJerry_General_09_DObj_SAND":
                    type = typeof(NpcVsJerry_General_09_DObj_SAND);
                    break;

                case "NpcVsJerry_General_18_DObj_SAND":
                    type = typeof(NpcVsJerry_General_18_DObj_SAND);
                    break;

                case "NpcVsJerry_General_NoFes_18_DObj_SAND":
                    type = typeof(NpcVsJerry_General_NoFes_18_DObj_SAND);
                    break;

                case "NpcVsJerry_Giftshop_01_DObj_SAND":
                    type = typeof(NpcVsJerry_Giftshop_01_DObj_SAND);
                    break;

                case "NpcVsJerry_Harbor02_02_DObj_SAND":
                    type = typeof(NpcVsJerry_Harbor02_02_DObj_SAND);
                    break;

                case "NpcVsJerry_Harbor02_02_DObj_SAND_Cstage":
                    type = typeof(NpcVsJerry_Harbor02_02_DObj_SAND_Cstage);
                    break;

                case "NpcVsJerry_Harbor02_03_DObj_SAND":
                    type = typeof(NpcVsJerry_Harbor02_03_DObj_SAND);
                    break;

                case "NpcVsJerry_HelmetN_10_DObj_SAND":
                    type = typeof(NpcVsJerry_HelmetN_10_DObj_SAND);
                    break;

                case "NpcVsJerry_Ramen_00_DObj_SAND":
                    type = typeof(NpcVsJerry_Ramen_00_DObj_SAND);
                    break;

                case "NpcVsJerry_Sightseer_00_DObj_SAND":
                    type = typeof(NpcVsJerry_Sightseer_00_DObj_SAND);
                    break;

                case "NpcVsJerry_TourGuide01_01_DObj_SAND":
                    type = typeof(NpcVsJerry_TourGuide01_01_DObj_SAND);
                    break;

                case "NpcVsJerry_TourGuide01_01_DObj_SAND_Cstage":
                    type = typeof(NpcVsJerry_TourGuide01_01_DObj_SAND_Cstage);
                    break;

                case "Npc_Commander_Dried_SAND":
                    type = typeof(Npc_Commander_Dried_SAND);
                    break;

                case "Npc_CustomShop_Fsodr":
                    type = typeof(Npc_CustomShop_Fsodr);
                    break;

                case "Npc_JerryFest_BadgeBag00_DObj_SAND":
                    type = typeof(Npc_JerryFest_BadgeBag00_DObj_SAND);
                    break;

                case "Npc_JerryFest_BadgeBag01_DObj_SAND":
                    type = typeof(Npc_JerryFest_BadgeBag01_DObj_SAND);
                    break;

                case "Npc_JerryFest_BadgeBag02_DObj_SAND":
                    type = typeof(Npc_JerryFest_BadgeBag02_DObj_SAND);
                    break;

                case "Npc_JerryFest_BagPV_DObj_SAND":
                    type = typeof(Npc_JerryFest_BagPV_DObj_SAND);
                    break;

                case "Npc_JerryFest_Box_DObj_SAND":
                    type = typeof(Npc_JerryFest_Box_DObj_SAND);
                    break;

                case "Npc_JerryFest_Fan00_DObj_SAND":
                    type = typeof(Npc_JerryFest_Fan00_DObj_SAND);
                    break;

                case "Npc_JerryFest_Fan01_DObj_SAND":
                    type = typeof(Npc_JerryFest_Fan01_DObj_SAND);
                    break;

                case "Npc_JerryFest_Fan02_DObj_SAND":
                    type = typeof(Npc_JerryFest_Fan02_DObj_SAND);
                    break;

                case "Npc_JerryFest_HeadBand00_DObj_SAND":
                    type = typeof(Npc_JerryFest_HeadBand00_DObj_SAND);
                    break;

                case "Npc_JerryFest_HeadBand01_DObj_SAND":
                    type = typeof(Npc_JerryFest_HeadBand01_DObj_SAND);
                    break;

                case "Npc_JerryFest_HeadBand02_DObj_SAND":
                    type = typeof(Npc_JerryFest_HeadBand02_DObj_SAND);
                    break;

                case "Npc_JerryFest_Impatience_DObj_SAND":
                    type = typeof(Npc_JerryFest_Impatience_DObj_SAND);
                    break;

                case "Npc_JerryFest_PlasticBag_DObj_SAND":
                    type = typeof(Npc_JerryFest_PlasticBag_DObj_SAND);
                    break;

                case "Npc_JerryFest_Shop00_00_DObj_SAND":
                    type = typeof(Npc_JerryFest_Shop00_00_DObj_SAND);
                    break;

                case "Npc_JerryFest_Shop00_02_DObj_SAND":
                    type = typeof(Npc_JerryFest_Shop00_02_DObj_SAND);
                    break;

                case "Npc_JerryFest_Shop00_02_DObj_SAND_01":
                    type = typeof(Npc_JerryFest_Shop00_02_DObj_SAND_01);
                    break;

                case "Npc_JerryFest_Stepladder_DObj_SAND":
                    type = typeof(Npc_JerryFest_Stepladder_DObj_SAND);
                    break;

                case "Npc_JerryFest_Towel_DObj_SAND":
                    type = typeof(Npc_JerryFest_Towel_DObj_SAND);
                    break;

                case "Npc_JerryFest_WeepingChild_DObj_SAND":
                    type = typeof(Npc_JerryFest_WeepingChild_DObj_SAND);
                    break;

                case "Npc_Mizuta_SAND":
                    type = typeof(Npc_Mizuta_SAND);
                    break;

                case "Npc_Mizuta_SAND_CStage":
                    type = typeof(Npc_Mizuta_SAND_CStage);
                    break;

                case "SandPlazaBandLocator":
                    type = typeof(SandPlazaBandLocator);
                    break;

                case "SandPlazaStageLocator":
                    type = typeof(SandPlazaStageLocator);
                    break;
                default:
                    type = typeof(MuObj);
                    break;
            }

            if (className.StartsWith("Lft_FldObj"))
            {
                type = typeof(Lft_FldObj_Generic);
            }
            if (className.StartsWith("DObj_FldObj"))
            {
                type = typeof(DObj_FldObj_Generic);
            }
            if (className.StartsWith("FldObj"))
            {
                type = typeof(FldObj_Generic);
            }

            Console.WriteLine($"  Using: {type.Name}");
        }

        // Regular Deserialize
        public static void Deserialize(object section, dynamic value)
        {
            if (value is IList)
            {
                var props = section.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                SetValues(props[0], props[0].PropertyType, section, value);
                return;
            }

            Dictionary<string, dynamic> bymlProperties;

            if (value is Dictionary<string, dynamic>) bymlProperties = (Dictionary<string, dynamic>)value;
            else throw new Exception("Not a dictionary");

            if (section is IByamlSerializable)
                ((IByamlSerializable)section).DeserializeByaml(bymlProperties);

            var properties = section.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var fields = section.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            for (int i = 0; i < fields.Length; i++)
            {
                //Only load properties with byaml attributes
                var byamlAttribute = fields[i].GetCustomAttribute<ByamlMember>();
                if (byamlAttribute == null)
                    continue;

                Type type = fields[i].FieldType;
                Type nullableType = Nullable.GetUnderlyingType(type);
                if (nullableType != null)
                    type = nullableType;

                //Set custom keys as property name if used
                string name = byamlAttribute.Key != null ? byamlAttribute.Key : fields[i].Name;

                //Skip properties that are not present
                if (!bymlProperties.ContainsKey(name))
                    continue;

                SetValues(fields[i], type, section, bymlProperties[name]);
            }

            for (int i = 0; i < properties.Length; i++)
            {
                //Get a property that stores the current dynamic dictionary
                var byamlPropertiesAttribute = properties[i].GetCustomAttribute<ByamlPropertyList>();
                if (byamlPropertiesAttribute != null)
                {
                    //Store the whole dynamic into the dictionary of the property
                    //All properties will be stored then saved back if none are serialized elsewhere in the class
                    properties[i].SetValue(section, value);
                    continue;
                }

                //Only load properties with byaml attributes
                var byamlAttribute = properties[i].GetCustomAttribute<ByamlMember>();
                if (byamlAttribute == null)
                    continue;

                Type type = properties[i].PropertyType;
                Type nullableType = Nullable.GetUnderlyingType(type);
                if (nullableType != null)
                    type = nullableType;

                //Set custom keys as property name if used
                string name = byamlAttribute.Key != null ? byamlAttribute.Key : properties[i].Name;

                //Skip properties that are not present
                if (!bymlProperties.ContainsKey(name))
                    continue;

                //Make sure the property has a setter and getter
                if (!properties[i].CanRead || !properties[i].CanWrite)
                {
                    throw new Exception(
                        $"Property {value}.{properties[i].Name} requires both a getter and setter to be used for dserialization.");
                }

                SetValues(properties[i], type, section, bymlProperties[name]);
            }
        }

        public static dynamic Serialize(object section)
        {
            return SetDictionary(section);
        }

        static dynamic SetDictionary(object section)
        {
            IDictionary<string, dynamic> bymlProperties = new Dictionary<string, dynamic>();

            if (section is IByamlSerializable)
                ((IByamlSerializable)section).SerializeByaml(bymlProperties);

            if (section is ByamlVector3F)
            {
                bymlProperties.Add("X", ((ByamlVector3F)section).X);
                bymlProperties.Add("Y", ((ByamlVector3F)section).Y);
                bymlProperties.Add("Z", ((ByamlVector3F)section).Z);
                return bymlProperties;
            }

            var properties = section.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var fields = section.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            for (int i = 0; i < fields.Length; i++)
            {
                //Only load properties with byaml attributes
                var byamlAttribute = fields[i].GetCustomAttribute<ByamlMember>();
                if (byamlAttribute == null)
                    continue;

                //Skip null optional values
                if (byamlAttribute.Optional && fields[i].GetValue(section) == null)
                    continue;

                //Set custom keys as property name if used
                string name = byamlAttribute.Key != null ? byamlAttribute.Key : fields[i].Name;
                bymlProperties.Add(name, SetBymlValue(fields[i].GetValue(section)));
            }

            for (int i = 0; i < properties.Length; i++)
            {
                //Only load properties with byaml attributes
                var byamlAttribute = properties[i].GetCustomAttribute<ByamlMember>();
                if (byamlAttribute == null)
                    continue;

                //Skip null optional values
                if (byamlAttribute.Optional && properties[i].GetValue(section) == null)
                    continue;

                //Skip empty lists
                if (typeof(IList).GetTypeInfo().IsAssignableFrom(properties[i].PropertyType) && ((IList)properties[i].GetValue(section)).Count == 0)
                    continue;

                //Set custom keys as property name if used
                string name = byamlAttribute.Key != null ? byamlAttribute.Key : properties[i].Name;

                //Make sure the property has a setter and getter
                if (!properties[i].CanRead || !properties[i].CanWrite)
                {
                    throw new Exception(
                        $"Property {section}.{properties[i].Name} requires both a getter and setter to be used for dserialization.");
                }

                bymlProperties.Add(name, SetBymlValue(properties[i].GetValue(section)));
            }

            return bymlProperties;
        }

        static dynamic SetBymlValue(object value)
        {
            Type type = value.GetType();
            Type nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null && nullableType.GetTypeInfo().IsEnum)
                type = nullableType;
            if (type.IsEnum)
                type = Enum.GetUnderlyingType(type);

            if (value is IList<ByamlExt.Byaml.ByamlPathPoint>)
                return value;

            if (type == typeof(bool)) return value;
            else if (type == typeof(float)) return value;
            else if (type == typeof(int)) return (int)value;
            else if (type == typeof(uint)) return (uint)value;
            else if (type == typeof(string)) return value;
            else if (type == typeof(ulong)) return (ulong)value;
            else if (type == typeof(decimal)) return value;
            else if (type == typeof(ByamlExt.Byaml.ByamlPathPoint)) return value;
            else if (typeof(IList).GetTypeInfo().IsAssignableFrom(type))
            {
                List<dynamic> savedValues = new List<dynamic>();
                foreach (var val in ((IList)value))
                    savedValues.Add(SetBymlValue(val));
                return savedValues;
            }
            else if (IsTypeByamlObject(type))
                return SetDictionary(value);

            throw new Exception($"Type {type.Name} is not supported as BYAML data.");
        }

        static bool IsTypeByamlObject(Type type)
        {
            return type.GetTypeInfo().GetCustomAttribute<ByamlObject>(true) != null;
        }

        static void SetValues(object property, Type type, object section, dynamic value)
        {
            //Console.WriteLine($"SetValues");
            if (value is IList<ByamlExt.Byaml.ByamlPathPoint>)
            {
                SetValue(property, section, value);
            }
            else if (value is IList<dynamic>)
            {
                var list = (IList<dynamic>)value;
                var array = InstantiateType<IList>(type);

                Type elementType = type.GetTypeInfo().GetElementType();
                if (type.IsGenericType && elementType == null)
                    elementType = type.GetGenericArguments()[0];

                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j] is IDictionary<string, dynamic>)
                    {
                        var instance = CreateInstance(elementType);
                        Deserialize(instance, list[j]);
                        array.Add(instance);
                    }
                    else if (list[j] is IList<dynamic>)
                    {
                        var subList = list[j] as IList<dynamic>;

                        var instance = CreateInstance(elementType);
                        if (instance is IList)
                        {
                            for (int k = 0; k < subList.Count; k++)
                                ((IList)instance).Add(subList[k]);
                        }
                        array.Add(instance);
                    }
                    else
                        array.Add(list[j]);
                }
                SetValue(property, section, array);
            }
            else if (value is IDictionary<string, dynamic>)
            {
                if (type == typeof(ByamlVector3F))
                {
                    var values = (IDictionary<string, dynamic>)value;
                    var vec3 = new ByamlVector3F(values["X"], values["Y"], values["Z"]);
                    SetValue(property, section, vec3);
                }
                else
                {
                    var instance = CreateInstance(type);
                    Deserialize(instance, value);
                    SetValue(property, section, instance);
                }
            }
            else
                SetValue(property, section, value);
        }

        static object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type, true);

            //return type.CreateInstance();
        }

        static void SetValue(object property, object instance, object value)
        {
            if (property is PropertyInfo)
            {
                Type nullableType = Nullable.GetUnderlyingType(((PropertyInfo)property).PropertyType);
                if (nullableType != null && nullableType.GetTypeInfo().IsEnum)
                {
                    value = Enum.ToObject(nullableType, value);
                }
            }

            if (property is PropertyInfo)
                ((PropertyInfo)property).SetValue(instance, value);
            else if (property is FieldInfo)
                ((FieldInfo)property).SetValue(instance, value);
        }

        private static bool IsTypeList(Type type)
        {
            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IList));
        }

        private static T InstantiateType<T>(Type type)
        {
            // Validate if the given type is compatible with the required one.
            if (!typeof(T).GetTypeInfo().IsAssignableFrom(type))
            {
                throw new Exception($"Type {type.Name} cannot be used as BYAML object data.");
            }
            // Return a new instance.
            return (T)CreateInstance(type);
        }
    }
}
