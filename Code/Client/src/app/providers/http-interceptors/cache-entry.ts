import { HttpResponse } from '@angular/common/http';

export interface CacheEntry {
  expires: number;
  event: HttpResponse<unknown>;
}
