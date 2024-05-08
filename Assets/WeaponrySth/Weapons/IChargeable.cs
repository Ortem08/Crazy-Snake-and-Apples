using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChargeable
{
    public ChargeInfo ChargeInfo { get; }

    public event Action<ChargeInfo> OnChargeChanged;
}
