import { Result } from 'ts-results-es';

export function parseJsonResult<T>(jsonValue: string): Result<T, unknown> {
  return Result.wrap(() => JSON.parse(jsonValue) as T);
}

export function decodeBase64JsonResult<T>(
  base64Value: string,
): Result<T, unknown> {
  return Result.wrap(() => JSON.parse(atob(base64Value)) as T);
}
