document.addEventListener("DOMContentLoaded", () => {
  initPreloader();
  initPageTransitions();
  initThemeToggle();
  initRevealAnimations();
  initAssessmentLoading();
  initSkeletonTables();
  initCountUp();
  initDashboardCharts();
  initResultCharts();
  initReviewForm();
});

function initPreloader() {
  const preloader = document.getElementById("appPreloader");

  if (!preloader) {
    return;
  }

  const hide = () => {
    preloader.classList.add("is-hidden");
    window.setTimeout(() => preloader.remove(), 520);
  };

  if (document.readyState === "complete") {
    window.setTimeout(hide, 350);
  } else {
    window.addEventListener("load", () => window.setTimeout(hide, 350), { once: true });
  }
}

function initPageTransitions() {
  const overlay = document.getElementById("pageTransition");

  if (!overlay) {
    return;
  }

  document.addEventListener("click", (event) => {
    const link = event.target.closest("a[href]");

    if (!link || link.target || link.hasAttribute("download")) {
      return;
    }

    const url = new URL(link.href, window.location.href);
    const isSamePageHash = url.pathname === window.location.pathname && url.hash;

    if (url.origin !== window.location.origin || isSamePageHash) {
      return;
    }

    event.preventDefault();
    overlay.classList.add("active");
    document.body.classList.add("is-transitioning");
    window.setTimeout(() => {
      window.location.href = url.href;
    }, 240);
  });
}

function initThemeToggle() {
  const toggle = document.getElementById("themeToggle");

  if (!toggle) {
    return;
  }

  const sync = () => {
    const theme = document.documentElement.getAttribute("data-theme") || "light";
    const text = toggle.querySelector(".theme-toggle-text");
    if (text) {
      text.textContent = theme === "dark" ? "Sáng" : "Tối";
    }
  };

  sync();

  toggle.addEventListener("click", () => {
    const current = document.documentElement.getAttribute("data-theme") || "light";
    const next = current === "dark" ? "light" : "dark";
    document.documentElement.setAttribute("data-theme", next);
    localStorage.setItem("heartcare-theme", next);
    sync();
    redrawAllCharts();
    showToast("Đã đổi giao diện", next === "dark" ? "Chế độ tối đã được bật." : "Chế độ sáng đã được bật.");
  });
}

function initRevealAnimations() {
  const elements = document.querySelectorAll(".reveal-on-scroll");

  if (!("IntersectionObserver" in window)) {
    elements.forEach((element) => element.classList.add("is-visible"));
    return;
  }

  const observer = new IntersectionObserver((entries) => {
    entries.forEach((entry) => {
      if (entry.isIntersecting) {
        entry.target.classList.add("is-visible");
        observer.unobserve(entry.target);
      }
    });
  }, { threshold: 0.12 });

  elements.forEach((element) => observer.observe(element));
}

function initAssessmentLoading() {
  const form = document.getElementById("riskForm");
  const overlay = document.getElementById("analysisOverlay");

  if (!form || !overlay) {
    return;
  }

  form.addEventListener("submit", () => {
    if (!form.checkValidity()) {
      return;
    }

    overlay.classList.add("active");
    overlay.setAttribute("aria-hidden", "false");
  });
}

function initSkeletonTables() {
  document.querySelectorAll("[data-skeleton-table]").forEach((table) => {
    window.setTimeout(() => table.classList.add("is-loaded"), 700);
  });
}

function initCountUp() {
  const counters = document.querySelectorAll(".count-up");

  if (!counters.length) {
    return;
  }

  const animateCounter = (counter) => {
    if (counter.dataset.counted === "true") {
      return;
    }

    counter.dataset.counted = "true";
    const target = Number(counter.dataset.count || "0");
    const hasDecimal = !Number.isInteger(target);
    const duration = 950;
    const start = performance.now();

    const tick = (now) => {
      const progress = Math.min((now - start) / duration, 1);
      const eased = easeOutCubic(progress);
      const value = target * eased;
      counter.textContent = hasDecimal ? value.toFixed(1) : Math.round(value).toString();

      if (progress < 1) {
        requestAnimationFrame(tick);
      }
    };

    requestAnimationFrame(tick);
  };

  if (!("IntersectionObserver" in window)) {
    counters.forEach(animateCounter);
    return;
  }

  const observer = new IntersectionObserver((entries) => {
    entries.forEach((entry) => {
      if (entry.isIntersecting) {
        animateCounter(entry.target);
        observer.unobserve(entry.target);
      }
    });
  }, { threshold: 0.35 });

  counters.forEach((counter) => observer.observe(counter));
}

function initDashboardCharts() {
  if (!window.heartCareDashboard) {
    return;
  }

  redrawAllCharts(true);

  let resizeTimer;
  window.addEventListener("resize", () => {
    window.clearTimeout(resizeTimer);
    resizeTimer = window.setTimeout(() => redrawAllCharts(false), 160);
  });
}

function initResultCharts() {
  if (!window.heartCareResult) {
    return;
  }

  redrawResultCharts(true);
}

function redrawAllCharts(animated = false) {
  const dashboard = window.heartCareDashboard;

  if (!dashboard) {
    return;
  }

  const draw = (progress) => {
    drawDonutChart("riskDonutChart", dashboard.riskDistribution, progress);
    drawLineChart("riskTrendChart", dashboard.riskTrend, "%", progress);
    drawBarChart("ageBarChart", dashboard.ageDistribution, "Lượt", progress);
    drawBarChart("probabilityBarChart", dashboard.probabilityDistribution, "Lượt", progress);
    drawBarChart("cholesterolBarChart", dashboard.cholesterolByAge, "mg/dL", progress);
    drawLineChart("bloodPressureLineChart", dashboard.bloodPressureByAge, "mmHg", progress);
    drawDonutChart("genderDonutChart", dashboard.genderDistribution, progress);
    drawBarChart("ecgBarChart", dashboard.ecgDistribution, "Lượt", progress);
    drawDonutChart("smokingDonutChart", dashboard.smokingDistribution, progress);
    drawDonutChart("bloodSugarDonutChart", dashboard.bloodSugarDistribution, progress);
  };

  animateChart(draw, animated);
}

function redrawResultCharts(animated = false) {
  const result = window.heartCareResult;

  if (!result) {
    return;
  }

  animateChart((progress) => {
    drawHorizontalBarChart("resultFactorChart", result.contributions, "điểm", progress);
  }, animated);
}

function animateChart(draw, animated) {
  if (!animated) {
    draw(1);
    return;
  }

  const start = performance.now();
  const duration = 900;

  const tick = (now) => {
    const progress = Math.min((now - start) / duration, 1);
    draw(easeOutCubic(progress));

    if (progress < 1) {
      requestAnimationFrame(tick);
    }
  };

  requestAnimationFrame(tick);
}

function prepareCanvas(id) {
  const canvas = document.getElementById(id);

  if (!canvas) {
    return null;
  }

  const rect = canvas.getBoundingClientRect();
  const width = rect.width || canvas.width;
  const height = rect.height || canvas.height;
  const dpr = window.devicePixelRatio || 1;
  canvas.width = width * dpr;
  canvas.height = height * dpr;

  const context = canvas.getContext("2d");
  context.scale(dpr, dpr);
  context.clearRect(0, 0, width, height);

  return { canvas, context, width, height };
}

function drawDonutChart(id, data, progress = 1) {
  const prepared = prepareCanvas(id);

  if (!prepared || !Array.isArray(data)) {
    return;
  }

  const { context, width, height } = prepared;
  const total = data.reduce((sum, item) => sum + Number(item.value), 0);
  const centerX = width * 0.38;
  const centerY = height * 0.52;
  const radius = Math.min(width, height) * 0.28;
  const innerRadius = radius * 0.58;

  if (total <= 0) {
    drawEmptyState(context, width, height);
    return;
  }

  let startAngle = -Math.PI / 2;
  data.forEach((item) => {
    const slice = (Number(item.value) / total) * Math.PI * 2 * progress;
    context.beginPath();
    context.moveTo(centerX, centerY);
    context.arc(centerX, centerY, radius, startAngle, startAngle + slice);
    context.closePath();
    context.fillStyle = item.color;
    context.fill();
    startAngle += (Number(item.value) / total) * Math.PI * 2;
  });

  context.beginPath();
  context.fillStyle = getThemeColor("--surface-strong");
  context.arc(centerX, centerY, innerRadius, 0, Math.PI * 2);
  context.fill();

  context.fillStyle = getThemeColor("--navy");
  context.font = "800 28px Segoe UI, Arial";
  context.textAlign = "center";
  context.fillText(Math.round(total * progress), centerX, centerY + 6);
  context.font = "700 13px Segoe UI, Arial";
  context.fillStyle = getThemeColor("--muted");
  context.fillText("lượt", centerX, centerY + 28);

  drawLegend(context, data, width * 0.68, height * 0.32, progress);
}

function drawBarChart(id, data, unit, progress = 1) {
  const prepared = prepareCanvas(id);

  if (!prepared || !Array.isArray(data)) {
    return;
  }

  const { context, width, height } = prepared;
  const values = data.map((item) => Number(item.value));
  const maxValue = Math.max(...values, 1);
  const chartLeft = 42;
  const chartRight = width - 22;
  const chartBottom = height - 48;
  const chartTop = 24;
  const barGap = 18;
  const slotWidth = (chartRight - chartLeft) / Math.max(data.length, 1);
  const barWidth = Math.max(24, slotWidth - barGap);

  drawAxis(context, chartLeft, chartTop, chartBottom, chartRight);

  data.forEach((item, index) => {
    const value = Number(item.value) * progress;
    const barHeight = ((chartBottom - chartTop) * value) / maxValue;
    const x = chartLeft + index * slotWidth + (slotWidth - barWidth) / 2;
    const y = chartBottom - barHeight;

    context.fillStyle = item.color || getThemeColor("--teal");
    roundRect(context, x, y, barWidth, barHeight, 7);
    context.fill();

    context.fillStyle = getThemeColor("--navy");
    context.font = "750 13px Segoe UI, Arial";
    context.textAlign = "center";
    context.fillText(formatValue(Number(item.value) * progress), x + barWidth / 2, y - 8);
    context.fillStyle = getThemeColor("--muted");
    context.font = "700 12px Segoe UI, Arial";
    wrapCanvasLabel(context, item.label, x + barWidth / 2, chartBottom + 20, slotWidth);
  });

  drawUnit(context, unit, width);
}

function drawLineChart(id, data, unit, progress = 1) {
  const prepared = prepareCanvas(id);

  if (!prepared || !Array.isArray(data)) {
    return;
  }

  const { context, width, height } = prepared;
  const values = data.map((item) => Number(item.value));
  const maxValue = Math.max(...values, 1);
  const minValue = Math.min(...values.filter((value) => value > 0), maxValue);
  const chartLeft = 42;
  const chartRight = width - 26;
  const chartBottom = height - 48;
  const chartTop = 24;
  const range = Math.max(maxValue - minValue, 1);

  drawAxis(context, chartLeft, chartTop, chartBottom, chartRight);

  const points = data.map((item, index) => {
    const x = chartLeft + index * ((chartRight - chartLeft) / Math.max(data.length - 1, 1));
    const animatedValue = minValue + (Number(item.value) - minValue) * progress;
    const y = chartBottom - ((animatedValue - minValue) / range) * (chartBottom - chartTop);
    return { x, y, item };
  });

  context.beginPath();
  points.forEach((point, index) => {
    if (index === 0) {
      context.moveTo(point.x, point.y);
    } else {
      context.lineTo(point.x, point.y);
    }
  });
  context.strokeStyle = "#2563eb";
  context.lineWidth = 4;
  context.stroke();

  points.forEach((point) => {
    context.beginPath();
    context.fillStyle = point.item.color || "#2563eb";
    context.arc(point.x, point.y, 6, 0, Math.PI * 2);
    context.fill();

    context.fillStyle = getThemeColor("--navy");
    context.font = "750 13px Segoe UI, Arial";
    context.textAlign = "center";
    context.fillText(formatValue(Number(point.item.value) * progress), point.x, point.y - 12);
    context.fillStyle = getThemeColor("--muted");
    context.font = "700 12px Segoe UI, Arial";
    context.fillText(point.item.label, point.x, chartBottom + 22);
  });

  drawUnit(context, unit, width);
}

function drawHorizontalBarChart(id, data, unit, progress = 1) {
  const prepared = prepareCanvas(id);

  if (!prepared || !Array.isArray(data)) {
    return;
  }

  const { context, width, height } = prepared;
  const top = 22;
  const left = 142;
  const right = width - 74;
  const rowHeight = Math.min(34, (height - top - 18) / Math.max(data.length, 1));
  const maxValue = Math.max(...data.map((item) => Number(item.value)), 1);

  data.forEach((item, index) => {
    const y = top + index * rowHeight;
    const value = Number(item.value) * progress;
    const barWidth = ((right - left) * value) / maxValue;

    context.fillStyle = getThemeColor("--muted");
    context.font = "700 12px Segoe UI, Arial";
    context.textAlign = "right";
    context.fillText(item.label, left - 12, y + 16);

    context.fillStyle = "rgba(161, 190, 201, .22)";
    roundRect(context, left, y + 4, right - left, 12, 6);
    context.fill();

    context.fillStyle = item.color || getThemeColor("--teal");
    roundRect(context, left, y + 4, barWidth, 12, 6);
    context.fill();

    context.fillStyle = getThemeColor("--navy");
    context.font = "750 12px Segoe UI, Arial";
    context.textAlign = "left";
    context.fillText(`${formatValue(Number(item.value) * progress)} ${unit}`, right + 10, y + 16);
  });
}

function drawAxis(context, left, top, bottom, right) {
  context.strokeStyle = getThemeColor("--line");
  context.lineWidth = 1;
  context.beginPath();
  context.moveTo(left, top);
  context.lineTo(left, bottom);
  context.lineTo(right, bottom);
  context.stroke();
}

function drawLegend(context, data, x, y, progress = 1) {
  data.forEach((item, index) => {
    const rowY = y + index * 30;
    context.fillStyle = item.color;
    roundRect(context, x, rowY - 12, 16, 16, 4);
    context.fill();
    context.fillStyle = getThemeColor("--muted");
    context.font = "700 13px Segoe UI, Arial";
    context.textAlign = "left";
    context.fillText(`${item.label}: ${formatValue(Number(item.value) * progress)}`, x + 24, rowY + 1);
  });
}

function drawUnit(context, unit, width) {
  context.fillStyle = getThemeColor("--muted");
  context.font = "700 12px Segoe UI, Arial";
  context.textAlign = "right";
  context.fillText(unit, width - 22, 16);
}

function drawEmptyState(context, width, height) {
  context.fillStyle = getThemeColor("--muted");
  context.font = "700 14px Segoe UI, Arial";
  context.textAlign = "center";
  context.fillText("Chưa có dữ liệu", width / 2, height / 2);
}

function roundRect(context, x, y, width, height, radius) {
  const safeRadius = Math.min(radius, width / 2, Math.abs(height) / 2);
  context.beginPath();
  context.moveTo(x + safeRadius, y);
  context.arcTo(x + width, y, x + width, y + height, safeRadius);
  context.arcTo(x + width, y + height, x, y + height, safeRadius);
  context.arcTo(x, y + height, x, y, safeRadius);
  context.arcTo(x, y, x + width, y, safeRadius);
  context.closePath();
}

function wrapCanvasLabel(context, text, x, y, maxWidth) {
  if (context.measureText(text).width <= maxWidth) {
    context.fillText(text, x, y);
    return;
  }

  const words = String(text).split(" ");
  let line = "";
  let lineIndex = 0;

  words.forEach((word) => {
    const testLine = line ? `${line} ${word}` : word;
    if (context.measureText(testLine).width > maxWidth && line) {
      context.fillText(line, x, y + lineIndex * 14);
      line = word;
      lineIndex += 1;
    } else {
      line = testLine;
    }
  });

  if (line) {
    context.fillText(line, x, y + lineIndex * 14);
  }
}

function initReviewForm() {
  const form = document.getElementById("reviewForm");
  const list = document.getElementById("reviewList");

  if (!form || !list) {
    return;
  }

  form.addEventListener("submit", (event) => {
    event.preventDefault();

    const data = new FormData(form);
    const name = String(data.get("reviewName") || "").trim();
    const rating = String(data.get("reviewRating") || "5");
    const message = String(data.get("reviewMessage") || "").trim();

    if (!name || !message) {
      showToast("Thiếu thông tin", "Vui lòng nhập tên và nhận xét.");
      return;
    }

    const card = document.createElement("article");
    card.className = "review-card";
    card.innerHTML = `<strong>${escapeHtml(name)}</strong><span>${escapeHtml(rating)} sao</span><p>${escapeHtml(message)}</p>`;
    list.prepend(card);
    form.reset();
    showToast("Cảm ơn bạn", "Đánh giá đã được thêm vào trang demo.");
  });
}

function showToast(title, message) {
  const host = document.getElementById("toastHost");

  if (!host) {
    return;
  }

  const toast = document.createElement("div");
  toast.className = "app-toast";
  toast.innerHTML = `<strong>${escapeHtml(title)}</strong><span>${escapeHtml(message)}</span>`;
  host.appendChild(toast);

  window.setTimeout(() => {
    toast.style.opacity = "0";
    toast.style.transform = "translateY(10px)";
    window.setTimeout(() => toast.remove(), 260);
  }, 3200);
}

function escapeHtml(value) {
  return String(value)
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}

function getThemeColor(name) {
  return getComputedStyle(document.documentElement).getPropertyValue(name).trim();
}

function formatValue(value) {
  if (!Number.isFinite(value)) {
    return "0";
  }

  return Number.isInteger(value) ? value.toString() : value.toFixed(1);
}

function easeOutCubic(value) {
  return 1 - Math.pow(1 - value, 3);
}
