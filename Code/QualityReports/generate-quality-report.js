const fs = require("fs");
const path = require("path");

const repositoryRoot = process.cwd();
const outputDirectory = path.join(
  repositoryRoot,
  "Code",
  "QualityReports",
  "html",
);
const sonarMeasuresPath = path.join(
  repositoryRoot,
  "Code",
  "QualityReports",
  "sonar-measures.json",
);
const clientCoveragePath = path.join(
  repositoryRoot,
  "Code",
  "Client",
  "coverage",
  "cobertura-coverage.xml",
);
const backendSummaryPath = path.join(
  repositoryRoot,
  "Code",
  "Tests",
  "coveragereport",
  "Summary.txt",
);

function readFile(filePath) {
  return fs.existsSync(filePath) ? fs.readFileSync(filePath, "utf8") : "";
}

function escapeHtml(value) {
  return String(value)
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#39;");
}

function percent(value) {
  const numericValue = Number(value);
  return Number.isFinite(numericValue)
    ? `${(numericValue * 100).toFixed(2)}%`
    : "No disponible";
}

function parseCoberturaSummary(xmlContent) {
  const lineRate = xmlContent.match(/line-rate="([^"]+)"/)?.[1];
  const branchRate = xmlContent.match(/branch-rate="([^"]+)"/)?.[1];
  const linesCovered = xmlContent.match(/lines-covered="([^"]+)"/)?.[1];
  const linesValid = xmlContent.match(/lines-valid="([^"]+)"/)?.[1];

  return {
    lines: percent(lineRate),
    branches: percent(branchRate),
    covered:
      linesCovered && linesValid
        ? `${linesCovered}/${linesValid}`
        : "No disponible",
  };
}

function parseReportGeneratorSummary(summaryContent) {
  const lines = summaryContent
    .split(/\r?\n/)
    .map((line) => line.trim())
    .filter(Boolean);

  return lines.length > 0 ? lines : ["No disponible"];
}

function parseSonarMeasures() {
  if (!fs.existsSync(sonarMeasuresPath)) return [];

  const rawJson = JSON.parse(readFile(sonarMeasuresPath));
  const measures = rawJson.component?.measures ?? [];

  return measures.map((measure) => ({
    metric: measure.metric,
    value: measure.value ?? measure.period?.value ?? "No disponible",
  }));
}

function renderMetricList(items) {
  return items
    .map(
      (item) => `
        <li>
          <span>${escapeHtml(item.metric)}</span>
          <strong>${escapeHtml(item.value)}</strong>
        </li>
      `,
    )
    .join("");
}

function renderTextList(items) {
  return items.map((item) => `<li>${escapeHtml(item)}</li>`).join("");
}

fs.mkdirSync(outputDirectory, { recursive: true });

const clientCoverage = parseCoberturaSummary(readFile(clientCoveragePath));
const backendSummary = parseReportGeneratorSummary(
  readFile(backendSummaryPath),
);
const sonarMeasures = parseSonarMeasures();
const sonarDashboardUrl =
  process.env.SONAR_HOST_URL && process.env.SONAR_PROJECT_KEY
    ? `${process.env.SONAR_HOST_URL}/dashboard?id=${encodeURIComponent(
        process.env.SONAR_PROJECT_KEY,
      )}`
    : "";

const html = `<!doctype html>
<html lang="es">
  <head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Reporte visual de calidad</title>
    <style>
      :root {
        color-scheme: light;
        --background: #f6f7f9;
        --surface: #ffffff;
        --ink: #111827;
        --muted: #5f6b7a;
        --border: #d8dee8;
        --accent: #facc15;
      }
      * {
        box-sizing: border-box;
      }
      body {
        margin: 0;
        background: var(--background);
        color: var(--ink);
        font-family:
          Inter, ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI",
          sans-serif;
      }
      main {
        width: min(1120px, calc(100% - 32px));
        margin: 0 auto;
        padding: 40px 0;
      }
      h1,
      h2,
      p {
        margin: 0;
      }
      h1 {
        font-size: 32px;
        line-height: 1.15;
      }
      h2 {
        font-size: 18px;
      }
      .header {
        display: flex;
        justify-content: space-between;
        gap: 24px;
        align-items: flex-end;
        margin-bottom: 24px;
      }
      .timestamp {
        color: var(--muted);
        font-size: 14px;
      }
      .grid {
        display: grid;
        grid-template-columns: repeat(3, minmax(0, 1fr));
        gap: 16px;
      }
      .card {
        background: var(--surface);
        border: 1px solid var(--border);
        border-radius: 8px;
        padding: 20px;
        min-height: 180px;
      }
      .metric {
        display: flex;
        justify-content: space-between;
        gap: 16px;
        padding: 10px 0;
        border-bottom: 1px solid var(--border);
      }
      .metric:last-child {
        border-bottom: 0;
      }
      ul {
        list-style: none;
        padding: 0;
        margin: 16px 0 0;
      }
      li {
        display: flex;
        justify-content: space-between;
        gap: 16px;
        padding: 8px 0;
        border-bottom: 1px solid var(--border);
      }
      li:last-child {
        border-bottom: 0;
      }
      a {
        color: #0f4c81;
        font-weight: 700;
        text-decoration: none;
      }
      .links {
        display: flex;
        flex-wrap: wrap;
        gap: 12px;
        margin-top: 16px;
      }
      .link {
        display: inline-flex;
        align-items: center;
        min-height: 40px;
        padding: 0 14px;
        border: 1px solid var(--border);
        border-radius: 999px;
        background: #fffbea;
      }
      @media (max-width: 900px) {
        .grid {
          grid-template-columns: 1fr;
        }
        .header {
          align-items: flex-start;
          flex-direction: column;
        }
      }
    </style>
  </head>
  <body>
    <main>
      <section class="header">
        <div>
          <h1>Reporte visual de calidad CI/CD</h1>
          <p class="timestamp">Generado: ${escapeHtml(new Date().toISOString())}</p>
        </div>
      </section>
      <section class="grid">
        <article class="card">
          <h2>Cobertura frontend</h2>
          <div class="metric"><span>Líneas</span><strong>${clientCoverage.lines}</strong></div>
          <div class="metric"><span>Ramas</span><strong>${clientCoverage.branches}</strong></div>
          <div class="metric"><span>Cubiertas</span><strong>${clientCoverage.covered}</strong></div>
          <div class="links">
            <a class="link" href="./frontend-coverage/index.html">Abrir HTML</a>
          </div>
        </article>
        <article class="card">
          <h2>Cobertura backend</h2>
          <ul>${renderTextList(backendSummary)}</ul>
          <div class="links">
            <a class="link" href="./backend-coverage/index.html">Abrir HTML</a>
          </div>
        </article>
        <article class="card">
          <h2>SonarQube</h2>
          ${
            sonarMeasures.length > 0
              ? `<ul>${renderMetricList(sonarMeasures)}</ul>`
              : '<p class="timestamp">Sin métricas locales. Configura SONAR_TOKEN y SONAR_PROJECT_KEY para consultar medidas.</p>'
          }
          <div class="links">
            ${
              sonarDashboardUrl
                ? `<a class="link" href="${escapeHtml(sonarDashboardUrl)}">Abrir dashboard</a>`
                : ""
            }
          </div>
        </article>
      </section>
    </main>
  </body>
</html>
`;

fs.writeFileSync(path.join(outputDirectory, "index.html"), html);
