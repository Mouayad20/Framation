using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DotController : MonoBehaviour , IDragHandler, IPointerClickHandler {

    public int id;
    public Action<DotController> onDragEvent;
    public Action<DotController> OnRightClickEvent;
    public Action<DotController> OnLeftClickEvent;

    public void OnDrag(PointerEventData eventData){
        if (eventData.pointerId == -1){
            onDragEvent?.Invoke(this);
        }
    }

    public void OnPointerClick(PointerEventData eventData){
        if (eventData.pointerId == -2){
            OnRightClickEvent?.Invoke(this);
        }
        else if (eventData.pointerId == -1){
            OnLeftClickEvent?.Invoke(this);
        }
    }

    public DotController Clone(GameObject dotPrefab ,Transform dotParent ){
        DotController dot =  Instantiate(dotPrefab , new Vector3(this.transform.position.x,this.transform.position.y,0), Quaternion.identity, dotParent).GetComponent<DotController>();
        dot.id = this.id;
        return dot;
    }
}
