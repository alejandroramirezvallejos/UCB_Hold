import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from '@app/providers/app.config';
import { AppComponent } from '@app/ui/app.component';
import { Result } from 'ts-results-es';

void Result.wrapAsync(() => bootstrapApplication(AppComponent, appConfig)).then(
  (result) => {
    if (result.isErr()) {
      throw result.error;
    }
  },
);
