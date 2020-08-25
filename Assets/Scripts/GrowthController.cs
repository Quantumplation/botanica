using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
enum DragState {
    None,
    Deciding_Seed,
    Deciding_Medium_Root,
    Dragging,
}

public enum PlantSectionType {
    None,
    Seed,
    SkinnyRoot,
    MediumRoot,
    SkinnyPlant,
}

// Struct for the inspector, cause lazy
[Serializable]
public struct PlantSectionStats {
    public PlantSectionType type;
    public GameObject prefab;
    public int cost;
}

public class GrowthController : MonoBehaviour
{
    public int minerals = 1000;
    public int water = 50;
    public float mineralUp = 0;
    public float mineralDown = 0;
    public float waterUp = 0;
    public float waterDown = 0;
    float rateTimer = 0;
    public Text mineralsText;
    public Text waterText;
    public Text heightText;
    public Text depthText;
    public PlantSectionStats[] plantSections;
    DragState state = DragState.None;
    Vector2 dragStart = Vector2.zero;
    Growable dragParent = null;
    float segmentTime = 0.0f;
    GameObject currentlyGrowing = null;
    GameObject[] impassables;
    public MouseScroll focus;
    public GameObject dirt;


    // Start is called before the first frame update
    void Start()
    {
        impassables = GameObject.FindGameObjectsWithTag("Impassable");
    }

    void LateUpdate() {
        heightText.text = $"Height: {focus.plantHeight:0.##}";
        depthText.text = $"Depth: {Mathf.Abs(focus.plantDepth):0.##}";
        // TODO: track rates
        mineralsText.text = $"Minerals: {minerals}";
        waterText.text = $"Water: {water}";
    }

    // Update is called once per frame
    void Update()
    {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var mousePos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        if(state == DragState.None && Input.GetMouseButton(0)) {
            var hits = new List<RaycastHit2D>();
            var cf = new ContactFilter2D();
            cf.NoFilter();
            int hitCount = Physics2D.CircleCast(mousePos, 0.1f, Vector2.zero, cf, hits);
            Vector2 startPoint = Vector2.zero;
            GameObject obj = null;
            PlantSectionType? plantType = null;
            // Find the highest priority growth thing (seed > medium)
            for(int i = 0; i < hitCount; i++) {
                var hit = hits[i];
                var iObj = hit.collider.gameObject;
                var growable = iObj.GetComponent<Growable>();
                if(growable && !obj) {
                    obj = iObj;
                    startPoint = hit.point;
                    plantType = growable.rootType;
                } else if(growable) {
                    if(growable.rootType == PlantSectionType.Seed || plantType != PlantSectionType.Seed) {
                        obj = iObj;
                        startPoint = hit.point;
                        plantType = growable.rootType;
                    }
                }
            }
            if(obj != null && plantType != null) {
                // First, check if it's the end point
                var spline = obj.GetComponent<SpriteShapeController>()?.spline;
                if(spline != null) {
                    var pointCount = spline.GetPointCount();
                    var lastWorld = obj.transform.position + spline.GetPosition(pointCount - 1);
                    var last = new Vector2(lastWorld.x, lastWorld.y);
                    if((last - mousePos).magnitude < 0.4f) {
                        // We're close enough to the end, we should just continue growing this one
                        state = DragState.Dragging;
                        currentlyGrowing = obj;
                        return;
                    }
                }
                switch(plantType) {
                    case PlantSectionType.Seed: {
                        // Unlock the camera if this is the first time we've clicked on a seed
                        focus.startupLock = false;
                        // If we're clicking on a seed, drop into the deciding state
                        state = DragState.Deciding_Seed;
                        dragStart = startPoint;
                        dragParent = obj.GetComponent<Growable>();
                        obj.GetComponent<Animator>().SetTrigger("StopWiggle");
                    } break;
                    case PlantSectionType.MediumRoot: {
                        state = DragState.Deciding_Medium_Root;
                        dragStart = startPoint;
                        dragParent = obj.GetComponent<Growable>();
                    } break;
                    default: {
                    } break;
                }
            }
            return;
        }
        if(state == DragState.Deciding_Seed && Input.GetMouseButton(0)) {
            var delta = mousePos - dragStart;
            if(delta.sqrMagnitude < .5f) {
                // Update the indicator position, or something?
            } else {
                // Decide what to spawn based on whether you went up or down
                if(delta.y <= 0) {
                    // They went down, spawn a root!
                    currentlyGrowing = SpawnOffshoot(dragParent, PlantSectionType.MediumRoot, dragStart, mousePos);
                    state = DragState.Dragging;
                } else {
                    currentlyGrowing = SpawnOffshoot(dragParent, PlantSectionType.SkinnyPlant, dragStart, mousePos);
                    state = DragState.Dragging;
                }
            }
            return;
        }
        if(state == DragState.Deciding_Medium_Root && Input.GetMouseButton(0)) {
            var delta = mousePos - dragStart;
            if(delta.sqrMagnitude < .5f) {
                // Update the indicator position, or something?
            } else {
                currentlyGrowing = SpawnOffshoot(dragParent, PlantSectionType.SkinnyRoot, dragStart, mousePos);
                state = DragState.Dragging;
            }
        }
        if(state == DragState.Dragging && Input.GetMouseButton(0)) {
            var growable = currentlyGrowing.GetComponent<Growable>().growable;
            if(!growable) {
                return;
            }
            var currentType = currentlyGrowing.GetComponent<Growable>().rootType;
            var spline = currentlyGrowing.GetComponent<SpriteShapeController>().spline;
            var pointCount = spline.GetPointCount();
            // Check the length of the last segment
            var basePosWorld = currentlyGrowing.transform.position;
            var lastWorld = spline.GetPosition(pointCount - 1);
            var prevWorld = spline.GetPosition(pointCount - 2);
            var basePos = new Vector2(basePosWorld.x, basePosWorld.y);
            var last = new Vector2(lastWorld.x, lastWorld.y);
            var prev = new Vector2(prevWorld.x, prevWorld.y);

            // Grow the spline slightly
            var growDir = mousePos - (basePos + last);
            segmentTime += Time.deltaTime;
            var currentCost = currentType == PlantSectionType.MediumRoot ? 30 : 5;
            if(growDir.sqrMagnitude > .001) {
                var growDirN = growDir.normalized;
                var growSpeed = currentType == PlantSectionType.MediumRoot ? 0.5f : 1.5f;
                var newPos = last + growDirN * growSpeed * Time.deltaTime;
                var newPosWorld = basePos + newPos;

                // Check for rocks
                bool blocked = false;
                foreach(var rock in impassables) {
                    var collider = rock.GetComponent<Collider2D>();
                    if(collider.OverlapPoint(newPosWorld)) {
                        newPosWorld = collider.ClosestPoint(basePos + last);
                        newPos = newPosWorld - basePos;
                    }
                }
                // If roots, make sure we stay below ground
                if(currentType == PlantSectionType.SkinnyRoot ||
                   currentType == PlantSectionType.MediumRoot
                ) {
                   newPosWorld = dirt.GetComponent<Collider2D>().ClosestPoint(basePos + newPos);
                   newPos = newPosWorld - basePos;
               }

                var newPosRel = new Vector3(newPos.x, newPos.y, 0.0f);
                
                if(newPosWorld.y > focus.plantHeight) {
                    focus.plantHeight = newPosWorld.y;
                }
                if(newPosWorld.y < focus.plantDepth) {
                    focus.plantDepth = newPosWorld.y;
                }

                var turningRadius = currentType == PlantSectionType.MediumRoot ? 0.7f : 0.3f;
                if((last - prev).sqrMagnitude < turningRadius && segmentTime < 2) {
                    spline.SetPosition(pointCount - 1, newPosRel);
                } else if(currentCost <= minerals) {
                    segmentTime = 0.0f;
                    var midpoint = (lastWorld + prevWorld) / 2;
                    spline.SetPosition(pointCount - 1, midpoint);
                    spline.InsertPointAt(pointCount, newPosRel);
                    // we just added something to the spline, so pointCount now refers to the last
                    minerals -= currentCost;
                    switch(currentType) {
                        case PlantSectionType.MediumRoot: {
                            spline.SetHeight(pointCount, .3f);
                        } break;
                        case PlantSectionType.SkinnyRoot: {
                            spline.SetHeight(pointCount, .1f);
                        } break;
                        case PlantSectionType.SkinnyPlant: {
                            spline.SetHeight(pointCount, .2f);
                        } break;
                    }
                    // pointCount - 1 is the second-to-last
                    spline.SetTangentMode(pointCount - 1, ShapeTangentMode.Continuous);
                    Vector3 right;
                    Vector3 left;
                    SplineUtility.CalculateTangents(
                            lastWorld, prevWorld, newPosRel, Vector2.zero,
                            .2f, out right, out left
                    );
                    spline.SetLeftTangent(pointCount - 1, left);
                    spline.SetRightTangent(pointCount - 1, right);
                    float length = 0;
                    for(var i = 0; i < pointCount - 1; i++) {
                        if(i > 0) {
                            length += (spline.GetPosition(i) - spline.GetPosition(i-1)).magnitude;
                        }
                        var maxH = currentType == PlantSectionType.MediumRoot ? 0.7f : 0.5f;
                        var h = spline.GetHeight(i);
                        var dH = (maxH - h) / 50f;
                        var newH = h + dH;
                        spline.SetHeight(i, newH);
                    }
                    var maxLength = currentType == PlantSectionType.SkinnyPlant ? 75 : 25;
                    if(length > maxLength) {
                        currentlyGrowing.GetComponent<Growable>().growable = false;
                    }
                }
            }
        }
        if(!Input.GetMouseButton(0)) {
            state = DragState.None;
            dragStart = Vector2.zero;
            currentlyGrowing = null;
        }
    }

    private GameObject SpawnOffshoot(Growable parent, PlantSectionType type, Vector2 start, Vector2 firstPoint) {
        GameObject prefab = null;
        int cost = 0;
        var stats = plantSections.First(x => x.type == type);
        minerals -= stats.cost;
        var shoot = Instantiate(stats.prefab, new Vector3(start.x, start.y, 0), Quaternion.identity);
        var growable = shoot.GetComponent<Growable>();
        growable.rootType = type;
        growable.parent = parent;
        var rc = shoot.GetComponent<ResourceCollector>();
        if(rc) {
            rc.growthController = this;
        }
        var wd = shoot.GetComponent<WaterDrainer>();
        if(wd) {
            wd.growthController = this;
        }
        var drowner = shoot.GetComponent<Drowner>();
        if(drowner) {
            drowner.growthController = this; // Yuck;
        }
        var firstPointRel = firstPoint - start;
        shoot.GetComponent<SpriteShapeController>().spline.SetPosition(1, new Vector3(firstPointRel.x, firstPointRel.y, 0));
        return shoot;
    }
}
