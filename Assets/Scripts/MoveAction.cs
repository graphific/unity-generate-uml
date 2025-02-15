﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveAction : BaseAction
{
    [SerializeField] private Animator unitAnimator;
    [SerializeField] private int maxMoveDistance = 4;

    private Vector3 targetPosition;
   

    protected override void Awake () {
        base.Awake ();

        targetPosition = transform.position;
    }

    private void Update () {

        if(!isActive) {
            return;
        }

        Vector3 moveDirection = ( targetPosition - transform.position ).normalized;

        float stoppingDistance = .1f;
        if ( Vector3.Distance ( transform.position, targetPosition ) > stoppingDistance ) {
            
            float moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            unitAnimator.SetBool ( "IsWalking", true );
        } else {
            unitAnimator.SetBool ( "IsWalking", false );
            isActive = false;
            onActionComplete ();//callback using delegate
        }

        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp ( transform.forward, moveDirection, rotateSpeed * Time.deltaTime );
    }

    public override void TakeAction ( GridPosition gridPosition, Action onActionComplete ) {
        this.onActionComplete = onActionComplete;
        this.targetPosition = LevelGrid.Instance.GetWorldPosition( gridPosition );
        isActive = true;
    }

    public override List<GridPosition> GetValidActionGridPositionList() {
        List<GridPosition> validActionGridPositionList = new List<GridPosition> ();

        GridPosition unitGridPosition = unit.GetGridPosition ();

        for( int x = -maxMoveDistance; x<=maxMoveDistance; x++ ) {
            for ( int z = -maxMoveDistance; z <= maxMoveDistance; z++ ) {
                GridPosition offsetGridPosition = new GridPosition ( x, z );
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if(! LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }
                if(unitGridPosition == testGridPosition) {
                    // Same grid position where the unit is already at
                    continue;
                }
                if(LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) {
                    //Grid Position already occupied with another unit
                    continue;
                }

                validActionGridPositionList.Add ( testGridPosition );
            }
        }

        return validActionGridPositionList;
    }

    public override string GetActionName () {
        return "Move";
    }

}
