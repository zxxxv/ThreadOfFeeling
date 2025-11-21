from __future__ import annotations
from dataclasses import dataclass
from collections import deque
from typing import Optional, Deque

from gesture import FrameFeatures

NONE = 'NONE'
LEFT = 'LEFT'
RIGHT = 'RIGHT'
BOTH = 'BOTH'

@dataclass
class HandsFilterConfig:
    stab_window: int = 7
    stab_radio: float = 0.6
    both_policy: str = "both"

class RuleBaseHandsFilter:

    def __init__(self, cfg: Optional[HandsFilterConfig] = None):
        self.cfg = cfg or HandsFilterConfig()
        self._buf: Deque[int] = deque(maxlen=self.cfg.stab_window)
        self.last_code: int = NONE

    def infer_raw(self, ff: FrameFeatures) -> int:
        left_present = ff.left is not None
        right_present = ff.right is not None

        if not left_present and not right_present:
            return NONE
        if left_present and not right_present:
            return LEFT
        if right_present and not left_present:
            return RIGHT
        if self.cfg.both_policy == "both":
            return BOTH
        
    def infer(self, ff: FrameFeatures) -> int:
        raw = self.infer_raw(ff)
        self._buf.append(raw)

        if len(self._buf) < self._buf.maxlen:
            self.last_code = raw
            return raw
        
        from collections import Counter
        cnt = Counter(self._buf)
        best, c = cnt.most_common(1)[0]

        if c >= int(self.cfg.stab_window * self.cfg.stab_radio):
            self.last_code = best
            return best
        else:
            self.last_code = raw
            return raw
