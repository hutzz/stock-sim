// Industry exposure radar: one axis per sector, plotted as a share of portfolio value.
let radarChart;
// Per-chart hover detail (the tickers inside each sector) kept out of the chart config
// so updates can swap it freely - same approach as lineGraph.js.
const radarHoldings = new WeakMap();

// A fixed 0-100% scale leaves a diversified portfolio huddled in the middle of the web,
// so the outer ring is set just above the largest sector instead.
const radarMax = (shares) => {
	const peak = Math.max(0, ...shares);
	if (peak <= 0) return 25;
	return Math.min(100, Math.ceil((peak * 1.15) / 5) * 5);
};

// One bullet line per ticker held in the sector; Chart.js renders array entries as rows.
const holdingsLines = (item) => {
	const symbols = radarHoldings.get(item.chart)?.[item.dataIndex] ?? [];
	return symbols.map((symbol) => `• ${symbol}`);
};

window.drawSectorRadar = (labels, shares, holdings) => {
	const ctx = document.getElementById("sectorRadar");
	if (!ctx) return;
	radarChart = new Chart(ctx, {
		type: "radar",
		data: {
			labels: labels,
			datasets: [
				{
					label: "Portfolio",
					data: shares,
					borderColor: CHART_ACCENT,
					backgroundColor: window.withAlpha("#4b73e3", 0.22),
					pointBackgroundColor: labels.map((label) => window.sectorColor(label)),
					pointBorderColor: "#1d1c1c",
					pointRadius: 5,
					pointHoverRadius: 7,
					borderWidth: 2,
				},
			],
		},
		options: {
			responsive: true,
			maintainAspectRatio: false,
			plugins: {
				legend: { display: false },
				tooltip: {
					...CHART_TOOLTIP,
					displayColors: false,
					callbacks: {
						title: (items) => items[0].label,
						label: (item) => `${Number(item.parsed.r).toFixed(1)}% of portfolio`,
						afterLabel: holdingsLines,
					},
				},
			},
			scales: {
				r: {
					beginAtZero: true,
					max: radarMax(shares),
					ticks: {
						color: CHART_TICK_COLOR,
						backdropColor: "transparent",
						font: { size: 10 },
						callback: (value) => `${value}%`,
					},
					grid: { color: CHART_AXIS_COLOR },
					angleLines: { color: CHART_AXIS_COLOR },
					pointLabels: {
						color: CHART_LABEL_COLOR,
						font: { size: 11 },
					},
				},
			},
		},
	});
	radarHoldings.set(radarChart, holdings);
};

window.updateSectorRadar = (labels, shares, holdings) => {
	if (!radarChart) return;
	radarChart.data.labels = labels;
	radarChart.data.datasets[0].data = shares;
	radarChart.data.datasets[0].pointBackgroundColor = labels.map((label) =>
		window.sectorColor(label),
	);
	radarChart.options.scales.r.max = radarMax(shares);
	radarHoldings.set(radarChart, holdings);
	radarChart.update();
};

