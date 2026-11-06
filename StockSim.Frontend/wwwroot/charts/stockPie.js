// Concentration doughnut: one slice per position, biggest first, so the ring is a map of
// where the money actually sits. Sector detail travels in the tooltip, not on the ring.
let stockPieChart;
// Per-slice sector names kept out of the chart config so updates can swap them freely -
// same approach as lineGraph.js.
const pieSectors = new WeakMap();

// Ranked slices get distinct hues by position, which guarantees two neighbouring slices
// are never the same colour even when holdings churn between loads.
const PIE_PALETTE = [
	"#4b73e3",
	"#5aa9d6",
	"#4fb3a3",
	"#8fae6a",
	"#c9a24b",
	"#d98c4f",
	"#b8574f",
	"#7f6fd0",
	"#9a7fa8",
	"#6f9bb5",
	"#5fc2c9",
	"#d98fa8",
];

const OTHER_LABEL = "Other";
const otherColor = "rgba(255, 255, 255, 0.18)";

const sliceColors = (labels) =>
	labels.map((label, index) =>
		label === OTHER_LABEL ? otherColor : PIE_PALETTE[index % PIE_PALETTE.length],
	);

window.drawStockPie = (labels, values, sectors) => {
	const ctx = document.getElementById("stockPie");
	if (!ctx) return;
	stockPieChart = new Chart(ctx, {
		type: "doughnut",
		data: {
			labels: labels,
			datasets: [
				{
					data: values,
					backgroundColor: sliceColors(labels),
					borderColor: "#1d1c1c",
					borderWidth: 2,
					hoverBorderColor: "#2a4a78",
				},
			],
		},
		options: {
			responsive: true,
			maintainAspectRatio: false,
			cutout: "58%",
			plugins: {
				// A doughnut cannot label its own slices, so the tickers live in the legend.
				legend: {
					position: "bottom",
					labels: {
						color: CHART_LABEL_COLOR,
						boxWidth: 10,
						boxHeight: 10,
						padding: 8,
						font: { size: 10 },
					},
				},
				tooltip: {
					...CHART_TOOLTIP,
					callbacks: {
						label: (item) => {
							const total = item.dataset.data.reduce((sum, v) => sum + v, 0);
							const share = total > 0 ? ((item.parsed / total) * 100).toFixed(1) : "0.0";
							return ` $${Number(item.parsed).toFixed(2)} (${share}%)`;
						},
						afterLabel: (item) => pieSectors.get(item.chart)?.[item.dataIndex] || "",
					},
				},
			},
		},
	});
	pieSectors.set(stockPieChart, sectors);
};

window.updateStockPie = (labels, values, sectors) => {
	if (!stockPieChart) return;
	stockPieChart.data.labels = labels;
	stockPieChart.data.datasets[0].data = values;
	stockPieChart.data.datasets[0].backgroundColor = sliceColors(labels);
	pieSectors.set(stockPieChart, sectors);
	stockPieChart.update();
};
