﻿using System;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;

namespace Euphoria.Physics.Internal.Callbacks;

internal struct NarrowPhaseCallbacks : INarrowPhaseCallbacks
{
    public Action<CollidableReference, CollidableReference> ContactGenerationCallback;
    
    public CollidableProperty<CollisionType> CollisionTypes;

    public NarrowPhaseCallbacks(Action<CollidableReference, CollidableReference> contactGenerationCallback)
    {
        ContactGenerationCallback = contactGenerationCallback;
        
        CollisionTypes = new CollidableProperty<CollisionType>();
    }
    
    public void Initialize(Simulation simulation)
    {
        CollisionTypes.Initialize(simulation);
    }

    public bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b, ref float speculativeMargin)
    {
        if (a.Mobility == CollidableMobility.Static && b.Mobility == CollidableMobility.Static)
            return false;
        
        ContactGenerationCallback.Invoke(a, b);
        Console.WriteLine($"{workerIndex}, {speculativeMargin}");
        
        return CollisionTypes[a] == CollisionType.Solid && CollisionTypes[b] == CollisionType.Solid;
    }

    public bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
    {
        return true;
    }
    
    public bool ConfigureContactManifold<TManifold>(int workerIndex, CollidablePair pair, ref TManifold manifold,
        out PairMaterialProperties pairMaterial) where TManifold : unmanaged, IContactManifold<TManifold>
    {
        pairMaterial.FrictionCoefficient = 1f;
        pairMaterial.MaximumRecoveryVelocity = 2f;
        pairMaterial.SpringSettings = new SpringSettings(30, 1);
        return true;
    }

    public bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB,
        ref ConvexContactManifold manifold)
    {
        return true;
    }

    public void Dispose()
    {
        CollisionTypes.Dispose();
    }
}