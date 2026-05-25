import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { importProvidersFrom, LOCALE_ID } from '@angular/core';
import { AppRoutingModule } from './app/app-routing.module';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { registerLocaleData } from '@angular/common';
import { JwtInterceptor } from './app/interceptors/jwt.interceptor';
import { ResultResponseInterceptor } from './app/interceptors/result-response.interceptor';
import localeEs from '@angular/common/locales/es';
registerLocaleData(localeEs);
bootstrapApplication(AppComponent, {
  providers: [
    importProvidersFrom(AppRoutingModule, HttpClientModule),
    { provide: LOCALE_ID, useValue: 'es' },
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor,           multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ResultResponseInterceptor, multi: true }
  ]
}).catch(err => console.error(err));
