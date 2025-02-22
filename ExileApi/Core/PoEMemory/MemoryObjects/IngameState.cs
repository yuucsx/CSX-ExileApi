using System;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using GameOffsets;

namespace ExileCore.PoEMemory.MemoryObjects
{
    public class IngameState : RemoteMemoryObject
    {
        private readonly CachedValue<Camera> _camera;
        private readonly CachedValue<float> _CurrentUElementPosX;
        private readonly CachedValue<float> _CurrentUElementPosY;
        private readonly CachedValue<DiagnosticInfoType> _DiagnosticInfoType;
        private readonly CachedValue<EntityLabelMapOffsets> _EntityLabelMap;
        private readonly CachedValue<DiagnosticElement> _FPSRectangle;
        private readonly CachedValue<DiagnosticElement> _FrameTimeRectangle;
        private readonly CachedValue<IngameData> _ingameData;
        private readonly CachedValue<IngameStateOffsets> _ingameState;
        private readonly CachedValue<IngameUIElements> _ingameUi;
        private readonly CachedValue<DiagnosticElement> _LatencyRectangle;
        private CachedValue<Element> _mouseActions;
        private readonly CachedValue<ServerData> _serverData;
        private readonly CachedValue<float> _TimeInGameF;
        private readonly CachedValue<Element> _UIHover;
        private readonly CachedValue<Element> _UIHoverTooltip;
        private readonly CachedValue<float> _UIHoverX;
        private readonly CachedValue<float> _UIHoverY;
        private readonly CachedValue<Element> _UIRoot;

        public IngameState()
        {
            _ingameState = new FrameCache<IngameStateOffsets>(() => M.Read<IngameStateOffsets>(Address /*+M.offsets.IgsOffsetDelta*/));

            _camera = new AreaCache<Camera>(
                () => GetObject<Camera>(Address + /*0x1258*/Extensions.GetOffset<IngameStateOffsets>(nameof(IngameStateOffsets.Camera))));

            _ingameData = new AreaCache<IngameData>(() => GetObject<IngameData>(_ingameState.Value.Data));
            _serverData = new AreaCache<ServerData>(() => GetObject<ServerData>(_ingameState.Value.ServerData));
            _ingameUi = new AreaCache<IngameUIElements>(() => GetObject<IngameUIElements>(_ingameState.Value.IngameUi));
            _UIRoot = new AreaCache<Element>(() => GetObject<Element>(_ingameState.Value.UIRoot));
            _UIHover = new FrameCache<Element>(() => GetObject<Element>(_ingameState.Value.UIHover));
            _UIHoverX = new FrameCache<float>(() => _ingameState.Value.UIHoverX);
            _UIHoverY = new FrameCache<float>(() => _ingameState.Value.UIHoverY);
            _UIHoverTooltip = new FrameCache<Element>(() => GetObject<Element>(_ingameState.Value.UIHoverTooltip));
            _CurrentUElementPosX = new FrameCache<float>(() => _ingameState.Value.CurentUElementPosX);
            _CurrentUElementPosY = new FrameCache<float>(() => _ingameState.Value.CurentUElementPosY);
            _DiagnosticInfoType = new FrameCache<DiagnosticInfoType>(() => (DiagnosticInfoType) _ingameState.Value.DiagnosticInfoType);

            _LatencyRectangle = new AreaCache<DiagnosticElement>(
                () => GetObject<DiagnosticElement>(
                    Address + Extensions.GetOffset<IngameStateOffsets>(nameof(IngameStateOffsets.LatencyRectangle))));

            _FrameTimeRectangle = new AreaCache<DiagnosticElement>(
                () => GetObject<DiagnosticElement>(Address + /*0x1628*/
                                                   +Extensions.GetOffset<IngameStateOffsets>(
                                                       nameof(IngameStateOffsets.FrameTimeRectangle))));

            _FPSRectangle = new AreaCache<DiagnosticElement>(
                () => GetObject<DiagnosticElement>(Address /*0x1870*/ +
                                                   Extensions.GetOffset<IngameStateOffsets>(nameof(IngameStateOffsets.FPSRectangle))));

            _TimeInGameF = new FrameCache<float>(() => _ingameState.Value.TimeInGameF);
            _EntityLabelMap = new AreaCache<EntityLabelMapOffsets>(() => M.Read<EntityLabelMapOffsets>(_ingameState.Value.EntityLabelMap));
        }

        public Camera Camera => _camera.Value;
        public IngameData Data => _ingameData.Value;
        public bool InGame => ServerData.IsInGame;
        public ServerData ServerData => _serverData.Value;
        public IngameUIElements IngameUi => _ingameUi.Value;
        public Element UIRoot => _UIRoot.Value;
        public Element UIHover => _UIHover.Value;
        public float UIHoverX => _UIHoverX.Value;
        public float UIHoverY => _UIHoverY.Value;
        public Element UIHoverTooltip => _UIHoverTooltip.Value;
        public float CurentUElementPosX => _CurrentUElementPosX.Value;
        public float CurentUElementPosY => _CurrentUElementPosY.Value;
        public long EntityLabelMap => _EntityLabelMap.Value.EntityLabelMap;
        public DiagnosticInfoType DiagnosticInfoType => _DiagnosticInfoType.Value;
        public DiagnosticElement LatencyRectangle => _LatencyRectangle.Value;
        public DiagnosticElement FrameTimeRectangle => _FrameTimeRectangle.Value;
        public DiagnosticElement FPSRectangle => _FPSRectangle.Value;
        public float CurLatency => LatencyRectangle.CurrValue;
        public float CurFrameTime => FrameTimeRectangle.CurrValue;
        public float CurFps => FPSRectangle.CurrValue;
        public TimeSpan TimeInGame => TimeSpan.FromSeconds(_ingameState.Value.TimeInGame);
        public float TimeInGameF => _TimeInGameF.Value;

        public void UpdateData()
        {
            _ingameData.ForceUpdate();
        }
    }
}
